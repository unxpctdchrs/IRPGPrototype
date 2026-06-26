using UnityEngine;
using Zenject;

public class DoorToSupernaturalWorld : MonoBehaviour, IInteractable
{
    private ISceneService _sceneService;

    [Inject]
    private void Constructor(ISceneService sceneService)
    {
        _sceneService = sceneService;
    }

    public Vector3 GetInteractableUIPosition()
    {
        return transform.position + Vector3.up * 1.5f;
    }

    public void OnInteractStart()
    {
        _sceneService.LoadScene(SceneType.WorldScene);
    }

    public void OnInteractStop()
    {
    }
}
