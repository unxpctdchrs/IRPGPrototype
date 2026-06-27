using UnityEngine;
using Zenject;

public class PauseController : MonoBehaviour
{
    private InputManager _inputManager;
    private ISceneService _sceneService;

    [Inject]
    public void Construct(InputManager inputManager, ISceneService sceneService)
    {
        _inputManager = inputManager;
        _sceneService = sceneService;
    }

    private void OnEnable()
    {
        if (_inputManager != null)
        {
            _inputManager.OnPauseToggle += HandlePause;
        }
    }

    private void OnDisable()
    {
        if (_inputManager != null)
        {
            _inputManager.OnPauseToggle -= HandlePause;
        }
    }

    private void HandlePause()
    {
        _inputManager.EnableUIControls();
        _sceneService.LoadSceneAdditive(SceneType.PauseMenu); 
    }
}