using UnityEngine;

public interface IInteractable
{
    void OnInteractStart();
    void OnInteractStop();
    string GetInteractText();
    Vector3 GetInteractableUIPosition();
}
