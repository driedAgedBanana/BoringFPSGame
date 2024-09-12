using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionScript : MonoBehaviour
{
    private Camera _Maincamera; // Main camera reference
    [SerializeField] private float rayLength; // Length of the interaction ray

    private CapsuleCollider playerCollider; // Reference to the player's capsule collider

    [Header("Interaction UI images")]
    public Image[] interactionIcons; // Array of UI images for interaction icons

    private enum interactionIconType
    {
        JumpPad,
        Chest,
        Count // Must be placed last to represent the total number of icon types
    }

    private void Start()
    {
        setAllIconsInactive(); // Ensure all icons are inactive at the start

        playerCollider = GetComponent<CapsuleCollider>(); // Get the player's capsule collider

        // Find the main camera among the child cameras
        Camera[] mainCam = GetComponentsInChildren<Camera>();
        foreach (Camera cam in mainCam)
        {
            if (cam.CompareTag("MainCamera"))
            {
                _Maincamera = cam;
            }
        }
    }

    private void Update()
    {
        scanInteraction(); // Continuously scan for interactions
    }

    private void scanInteraction()
    {
        RaycastHit hit;
        Ray ray = _Maincamera.ScreenPointToRay(Input.mousePosition); // Create a ray from the camera through the mouse position

        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red); // Debugging line to visualize the ray

        // Check if the ray hits any collider within the rayLength
        if (Physics.Raycast(ray, out hit, rayLength))
        {
            string InteractionTag = hit.collider.tag; // Get the tag of the hit object

            // Determine which icon to show based on the hit object's tag
            switch (InteractionTag)
            {
                case "JumpPad":
                    SetIconsActive((int)interactionIconType.JumpPad);
                    break;

                case "Chest":
                    SetIconsActive((int)interactionIconType.Chest);
                    break;

                default:
                    setAllIconsInactive(); // Hide all icons if no recognizable tag is hit
                    break;
            }
        }
        else
        {
            setAllIconsInactive(); // Hide all icons if no object is hit
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag; // Get the tag of the object entering the trigger

        // Show appropriate icon based on the tag
        switch (tag)
        {
            case "JumpPad":
                SetIconsActive((int)interactionIconType.JumpPad);
                break;

            case "Chest":
                SetIconsActive((int)interactionIconType.Chest);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        string tag = other.gameObject.tag; // Get the tag of the object exiting the trigger

        // Hide all icons when exiting the trigger
        switch (tag)
        {
            case "JumpPad":
                setAllIconsInactive();
                break;

            case "Chest":
                setAllIconsInactive();
                break;
        }
    }

    private void SetIconsActive(int index)
    {
        setAllIconsInactive(); // Ensure all icons are inactive before activating a specific one

        // Activate the icon based on the provided index
        if (index >= 0 && index < interactionIcons.Length)
        {
            interactionIcons[index].gameObject.SetActive(true);
        }
    }

    private void setAllIconsInactive()
    {
        // Deactivate all interaction icons
        foreach (Image icon in interactionIcons)
        {
            icon.gameObject.SetActive(false);
        }
    }
}
