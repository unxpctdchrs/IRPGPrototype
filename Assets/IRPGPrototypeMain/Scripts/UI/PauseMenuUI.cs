using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _quitButton;

    private ISceneService _sceneService;
    private InputManager _inputManager;

    [Inject]
    public void Construct(ISceneService sceneService, InputManager inputManager)
    {
        _sceneService = sceneService;
        _inputManager = inputManager;
    }

    void Start()
    {
        if (_resumeButton != null) _resumeButton.onClick.AddListener(OnResumeButtonClicked);
        if (_mainMenuButton != null) _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        if (_quitButton != null) _quitButton.onClick.AddListener(OnQuitButtonClicked);

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

    private void OnMainMenuButtonClicked()
    {
        Time.timeScale = 1f;

        if (_sceneService != null)
        {
            _sceneService.LoadScene(SceneType.MainMenuScene);
        }
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        if (_inputManager != null) _inputManager.OnCancel += OnResumeButtonClicked;
    }

    private void OnDisable()
    {
        if (_inputManager != null) _inputManager.OnCancel -= OnResumeButtonClicked;
    }
}
