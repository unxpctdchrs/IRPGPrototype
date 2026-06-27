using UnityEngine;
using Zenject;

public class DoorToSupernaturalWorld : MonoBehaviour, IInteractable
{
    private ISceneService _sceneService;
    private ScenePayload _scenePayload;
    [SerializeField] private Vector3 _uiOffset = new Vector3(0.38f, 0.38f, 0);

    [Inject]
    private void Constructor(ISceneService sceneService, ScenePayload scenePayload)
    {
        _sceneService = sceneService;
        _scenePayload = scenePayload;
    }

    public Vector3 GetInteractableUIPosition() => transform.position + _uiOffset;

    public void OnInteractStart()
    {
        if (!_scenePayload.HasPlayedShedIntro)
        {
            _scenePayload.HasPlayedShedIntro = true;
        }
        _scenePayload.DestinationScene = SceneType.WorldScene;
        _sceneService.LoadScene(SceneType.TransitionScene);
    }

    public void OnInteractStop()
    {
    }

    public string GetInteractText()
    {
        return "Enter Supernatural World";
    }
}
