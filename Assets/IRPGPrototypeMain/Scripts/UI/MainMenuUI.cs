using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _quitButton;

    private ISceneService _sceneService;

    [Inject]
    public void Construct(ISceneService sceneService)
    {
        _sceneService = sceneService;
    }

    void Start()
    {
        _startGameButton.onClick.AddListener(OnStartGameClicked);
        _quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnStartGameClicked()
    {
        _sceneService.LoadScene(SceneType.DebugScene);
    }

    private void OnQuitClicked()
    {
        Application.Quit();
    } 
}
