using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCameraTransform;
    void Start()
    {
       //Find the main camera
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
       // Force the object to always face the camera
       if (mainCameraTransform != null)
        {
            transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward, mainCameraTransform.rotation * Vector3.up);

        }
    }
}
