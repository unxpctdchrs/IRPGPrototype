using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class SceneAudioCoordinator : MonoBehaviour
{
    [Header("Scene Ambience Setup")]
    [SerializeField] private AudioResource _mainMenuAmbience;
    [SerializeField] private AudioResource _shedAmbience;
    [SerializeField] private AudioResource _worldAmbience;

    private ISceneService _sceneService;
    private AudioManager _audioManager;

    [Inject]
    public void Construct(ISceneService sceneService, AudioManager audioManager)
    {
        _sceneService = sceneService;
        _audioManager = audioManager;
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
                _audioManager.PlayAmbience(_mainMenuAmbience);
                break;

            case SceneType.ShedScene:
                _audioManager.PlayAmbience(_shedAmbience);
                break;

            case SceneType.WorldScene:
                _audioManager.PlayAmbience(_worldAmbience);
                break;
        }
    }
}