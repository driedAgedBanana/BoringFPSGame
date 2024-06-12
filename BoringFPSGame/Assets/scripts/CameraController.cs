using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float SenX;
    [SerializeField] private float SenY;

    [SerializeField] Transform camera;
    [SerializeField] private Transform orientation;

    private float mouseX;
    private float mouseY;

    private float CameraMultiplier = 0.01f;

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        CameraInput();

        camera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
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
