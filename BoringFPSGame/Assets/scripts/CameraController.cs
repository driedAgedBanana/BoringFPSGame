using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float SenX;
    [SerializeField] private float SenY;

    Camera camera;

    private float mouseX;
    private float mouseY;

    private float CameraMultiplier = 0.01f;

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        camera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        CameraInput();

        camera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        camera.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void CameraInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * SenX * CameraMultiplier;
        xRotation -= mouseY * SenY * CameraMultiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }
}
