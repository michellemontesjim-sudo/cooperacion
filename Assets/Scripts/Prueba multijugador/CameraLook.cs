using System;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private Transform mainCameraTransform;

    void Awake()
    {
        mainCameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Makes the UI face the central camera perfectly
        transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                         mainCameraTransform.rotation * Vector3.up);


    }
}
