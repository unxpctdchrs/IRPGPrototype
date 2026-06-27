using System.Collections;
using UnityEngine;
using Zenject;

public class TransitionScene : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _transitionHoldTime = 4.5f; 

    private ScenePayload _payload;
    private ISceneService _sceneService;
    private SkyboxController _skyboxController;

    [Inject]
    public void Construct(ScenePayload payload, ISceneService sceneService, SkyboxController skyboxController)
    {
        _payload = payload;
        _sceneService = sceneService;
        _skyboxController = skyboxController;
    }

    private void Start()
    {
        StartCoroutine(TransitionSequenceRoutine());
    }

    private IEnumerator TransitionSequenceRoutine()
    {
        switch (_payload.DestinationScene)
        {
            case SceneType.WorldScene:
                _skyboxController.TriggerSpookyEnvironment();
                break;
            
            case SceneType.ShedScene:
            default:
                _skyboxController.TriggerNormalEnvironment();
                break;
        }

        yield return new WaitForSeconds(_transitionHoldTime);
        _sceneService.LoadScene(_payload.DestinationScene);
    }
}