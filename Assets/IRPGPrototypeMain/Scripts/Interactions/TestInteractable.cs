using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    public Vector3 GetInteractableUIPosition()
    {
        return transform.position + Vector3.up * 1.5f;
    }

    public string GetInteractText()
    {
        return "Test";
    }

    public void OnInteractStart()
    {
        Debug.Log("[TestInteractable] its working");
    }

    public void OnInteractStop()
    {
        
    }
}
