using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quick script to move camera with mouse
/// </summary>
public class CameraControl : MonoBehaviour
{
    [SerializeField]
    private float sensitivity = 20f;
    [SerializeField]
    private float maxYAngle = 80f;

    private Transform playerTransform;
    private Vector2 xRot;
    private Vector2 yRot;

    private void Awake()
    {
        playerTransform = transform.parent;
    }

    void FixedUpdate()
    {
        xRot.x += Input.GetAxis("Mouse X") * sensitivity;
        xRot.x = Mathf.Repeat(xRot.x, 360);

        yRot.y -= Input.GetAxis("Mouse Y") * sensitivity;
        yRot.y = Mathf.Clamp(yRot.y, -maxYAngle, maxYAngle);

        transform.localRotation = Quaternion.Euler(yRot.y, 0, 0);
        playerTransform.localRotation = Quaternion.Euler(0, xRot.x, 0);
        if (Input.GetMouseButtonDown(0))
            Cursor.lockState = CursorLockMode.Locked;
    }
}
