using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneService : ISceneService
{
    private const string BASE_SCENE = "BaseScene";
    public SceneType CurrentScene { get; private set; }
    public event Action<SceneType> OnSceneLoaded;

    private readonly CoroutineRunner _coroutineRunner;
    // there is no [Inject] attribute because this class doesn't inherit from MonoBehaviour
    public SceneService(CoroutineRunner coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
    }

    public void LoadScene(SceneType sceneType)
    {
        _coroutineRunner.Run(LoadSceneCoroutine(sceneType));
    }

    public void LoadScene(string sceneName)
    {
        if (System.Enum.TryParse<SceneType>(sceneName, out var sceneType))
            LoadScene(sceneType);
        else
            Debug.LogError($"[SceneService] Unknown scene: {sceneName}");
    }

    private IEnumerator LoadSceneCoroutine(SceneType sceneType)
    {
        if (sceneType == SceneType.WorldScene)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (sceneType == SceneType.MainMenuScene)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        int sceneCount = SceneManager.sceneCount;
        for (int i = sceneCount - 1; i >= 0; i--)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != BASE_SCENE)
            {
                var unloadOp = SceneManager.UnloadSceneAsync(scene);
                while (!unloadOp.isDone) yield return null;
            }
        }

        yield return Resources.UnloadUnusedAssets();

        var loadOp = SceneManager.LoadSceneAsync(sceneType.ToString(), LoadSceneMode.Additive);
        while (!loadOp.isDone) yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneType.ToString()));
        CurrentScene = sceneType;
        OnSceneLoaded?.Invoke(sceneType);
    }

    public void UnloadScene(SceneType sceneType)
    {
        UnloadScene(sceneType.ToString());
    }

    public void UnloadScene(string sceneName)
    {
        _coroutineRunner.Run(UnloadSceneCoroutine(sceneName));
    }

    private IEnumerator UnloadSceneCoroutine(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        
        if (scene.isLoaded)
        {
            var unloadOp = SceneManager.UnloadSceneAsync(scene);
            while (unloadOp != null && !unloadOp.isDone) yield return null;
            
            Debug.Log($"[SceneService] Successfully unloaded additive scene: {sceneName}");
        }
        else
        {
            Debug.LogWarning($"[SceneService] Attempted to unload {sceneName}, but it is not currently loaded.");
        }
    }

    public void LoadSceneAdditive(SceneType sceneType)
    {
        _coroutineRunner.Run(LoadSceneAdditiveCoroutine(sceneType));
    }

    public void LoadSceneAdditive(string sceneName)
    {
        if (System.Enum.TryParse<SceneType>(sceneName, out var sceneType))
        {
            LoadSceneAdditive(sceneType);
        }
        else
        {
            Debug.LogError($"[SceneService] Unknown scene for additive load: {sceneName}");
        }
    }

    private IEnumerator LoadSceneAdditiveCoroutine(SceneType sceneType)
    {
        string sceneString = sceneType.ToString();

        if (SceneManager.GetSceneByName(sceneString).isLoaded)
        {
            Debug.LogWarning($"[SceneService] {sceneString} is already loaded additively.");
            yield break;
        }

        var loadOp = SceneManager.LoadSceneAsync(sceneString, LoadSceneMode.Additive);
        while (loadOp != null && !loadOp.isDone) yield return null;

        Debug.Log($"[SceneService] Successfully loaded additive scene: {sceneString}");
    }
}
