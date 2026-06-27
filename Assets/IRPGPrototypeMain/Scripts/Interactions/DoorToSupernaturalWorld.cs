using UnityEngine;
using Zenject;

public class DoorToSupernaturalWorld : MonoBehaviour, IInteractable
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
