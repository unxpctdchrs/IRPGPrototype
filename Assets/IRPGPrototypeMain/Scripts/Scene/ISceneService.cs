using System;

public enum SceneType 
{ 
    DebugScene,
    MainMenuScene,
    ShedScene, 
    WorldScene,
    BattleScene,
    TransitionScene,
    PauseMenu,
    ThankYou
}

public interface ISceneService
{
    event Action<SceneType> OnSceneLoaded;
    SceneType CurrentScene { get; }
    void LoadScene(SceneType sceneType);
    void LoadScene(string sceneName);
    void UnloadScene(SceneType sceneType);
    void UnloadScene(string sceneName);
    void LoadSceneAdditive(SceneType sceneType);
    void LoadSceneAdditive(string sceneName);
}
