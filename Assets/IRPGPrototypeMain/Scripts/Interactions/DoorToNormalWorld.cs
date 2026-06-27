using UnityEngine;
using Zenject;

public class DoorToNormalWorld : MonoBehaviour, IInteractable
{
    private ISceneService _sceneService;
    private ScenePayload _scenePayload;

    [Inject]
    private void Constructor(ISceneService sceneService, ScenePayload scenePayload)
    {
        _sceneService = sceneService;
        _scenePayload = scenePayload;
    }

    public Vector3 GetInteractableUIPosition()
    {
        return transform.position + Vector3.up * 1.5f;
    }

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
