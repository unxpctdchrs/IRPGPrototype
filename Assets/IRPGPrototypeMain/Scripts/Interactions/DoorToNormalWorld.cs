using UnityEngine;
using Zenject;

public class DoorToNormalWorld : MonoBehaviour, IInteractable
{
    private ISceneService _sceneService;
    private ScenePayload _scenePayload;
    [SerializeField] private Vector3 _uiOffset = new Vector3(0, 0.38f, -0.38f);

    [Inject]
    private void Constructor(ISceneService sceneService, ScenePayload scenePayload)
    {
        _sceneService = sceneService;
        _scenePayload = scenePayload;
    }

    public Vector3 GetInteractableUIPosition() => transform.position + _uiOffset;

    public void OnInteractStart()
    {
        _scenePayload.DestinationScene = SceneType.ShedScene;
        _sceneService.LoadScene(SceneType.TransitionScene);
    }

    public void OnInteractStop()
    {
    }

    public string GetInteractText()
    {
        return "Enter Normal World";
    }
}
