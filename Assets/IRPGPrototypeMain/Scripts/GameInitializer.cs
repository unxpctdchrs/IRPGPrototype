using UnityEngine;
using Zenject;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private SceneType _startScene = SceneType.MainMenuScene;

    private ISceneService _sceneService;

    [Inject]
    private void Construct(ISceneService sceneService)
    {
        _sceneService = sceneService;
    }

    void Start()
    {
        _sceneService.LoadScene(_startScene);
    }
}
