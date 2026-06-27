using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class SceneAudioCoordinator : MonoBehaviour
{
    private ISceneService _sceneService;
    private AudioManager _audioManager;
    private AudioLibrary _audioLibrary;

    [Inject]
    public void Construct(ISceneService sceneService, AudioManager audioManager, AudioLibrary audioLibrary)
    {
        _sceneService = sceneService;
        _audioManager = audioManager;
        _audioLibrary = audioLibrary;
    }

    private void OnEnable()
    {
        if (_sceneService != null)
            _sceneService.OnSceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        if (_sceneService != null)
            _sceneService.OnSceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.MainMenuScene:
                break;

            case SceneType.ShedScene:
                _audioManager.CrossfadeSceneAmbience(_audioLibrary.ShedAmbience);
                break;

            case SceneType.WorldScene:
                _audioManager.CrossfadeSceneAmbience(_audioLibrary.WorldAmbience);
                break;

            case SceneType.BattleScene:
                _audioManager.CrossfadeSceneAmbience(_audioLibrary.WorldAmbience); 
                _audioManager.PlayMusic(_audioLibrary.BattleMusic);
                break;
        }
    }
}