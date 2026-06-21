using UnityEngine;

public interface IInteractable
{
    void OnInteractStart();
    void OnInteractStop();
    Vector3 GetInteractableUIPosition();
}
