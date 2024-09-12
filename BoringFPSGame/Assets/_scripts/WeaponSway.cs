using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float Drag = 2.5f;           // Sensitivity of the weapon sway
    public float DragThreshold = -5f;   // Threshold for limiting sway
    public float Smoothness = 5f;       // Smoothness of the sway effect
    public Transform Parent;            // Reference to the parent transform (e.g., the camera)

    private Quaternion localRotation;   // Initial local rotation of the weapon

    // Start is called before the first frame update
    void Start()
    {
        localRotation = transform.localRotation; // Capture the initial rotation of the weapon
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse input for sway effect
        float z = (Input.GetAxis("Mouse Y")) * Drag; // Vertical mouse input
        float y = (Input.GetAxis("Mouse X")) * Drag; // Horizontal mouse input

        // Clamp the sway values to the threshold
        if (Drag >= 0)
        {
            y = (y > DragThreshold) ? DragThreshold : y; // Limit y to DragThreshold
            y = (y < -DragThreshold) ? -DragThreshold : y; // Limit y to -DragThreshold

            z = (z > DragThreshold) ? DragThreshold : z; // Limit z to DragThreshold
            z = (z < -DragThreshold) ? -DragThreshold : z; // Limit z to -DragThreshold
        }
        else
        {
            y = (y < DragThreshold) ? DragThreshold : y; // Limit y to DragThreshold
            y = (y > -DragThreshold) ? -DragThreshold : y; // Limit y to -DragThreshold

            z = (z < DragThreshold) ? -DragThreshold : z; // Limit z to DragThreshold
            z = (z > -DragThreshold) ? -DragThreshold : z; // Limit z to -DragThreshold
        }

        // Create a new rotation based on the mouse input
        Quaternion newRotation = Quaternion.Euler(
            localRotation.x,
            localRotation.y + y,
            Parent.localEulerAngles.z + localRotation.z + z
        );

        // Smoothly transition to the new rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, (Time.deltaTime * Smoothness));
    }
}
