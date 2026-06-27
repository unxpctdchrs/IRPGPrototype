using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ThankYou : MonoBehaviour
{
    [SerializeField] private Button _resumeButton;

    private InputManager _inputManager;
    private ISceneService _sceneService;

    [Inject]
    public void Construct(InputManager inputManager, ISceneService sceneService)
    {
        _inputManager = inputManager;
        _sceneService = sceneService;
    }

    void Start()
    {
        if (_resumeButton != null) _resumeButton.onClick.AddListener(OnResumeButtonClicked);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnResumeButtonClicked()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (_inputManager != null)
        {
            _inputManager.EnablePlayerControls();
        }

        if (_sceneService != null)
        {
            _sceneService.UnloadScene(SceneType.PauseMenu); 
        }
    }
}
