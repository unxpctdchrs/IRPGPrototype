using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCam;
    private float xRotation = 8f;

    void Start()
    {
        mainCam = Camera.main.transform;
    }

    void LateUpdate()
    {
        Vector3 forward = mainCam.forward;
        forward.y = 0;
        transform.rotation = Quaternion.LookRotation(forward) * Quaternion.Euler(xRotation, 0f, 0f);
    }
}