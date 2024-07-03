using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionScript : MonoBehaviour
{
    private Camera _Maincamera;
    [SerializeField] private float rayLength;

    private CapsuleCollider playerCollider;

    [Header("Interaction UI images")]
    public Image[] interactionIcons;

    private enum interactionIconType
    {
        Interactable, // test
        Count // must be placed last
    }

    private void Start()
    {
        setAllIconsInactive(); // Ensure all icons are inactive at the start

        playerCollider = GetComponent<CapsuleCollider>();

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
        scanInteraction();
    }

    private void scanInteraction()
    {
        RaycastHit hit;
        Ray ray = _Maincamera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);

        if (Physics.Raycast(ray, out hit, rayLength))
        {
            string InteractionTag = hit.collider.tag;

            switch (InteractionTag)
            {
                case "Interactable":
                    SetIconsActive((int)interactionIconType.Interactable);
                    break;

                // Add more if you want to

                default:
                    setAllIconsInactive();
                    break;
            }
        }
        else
        {
            setAllIconsInactive(); // Ensure icons are turned off when nothing is hit
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;

        switch (tag)
        {
            case "Interactable":
                SetIconsActive((int)interactionIconType.Interactable);
                break;
                // Add more interactions in case you need it
        }
    }

    private void OnTriggerExit(Collider other)
    {
        string tag = other.gameObject.tag;

        switch (tag)
        {
            case "Interactable":
                // Add more tags if needed
                setAllIconsInactive();
                break;
        }
    }

    private void SetIconsActive(int index)
    {
        setAllIconsInactive();

        if (index >= 0 && index < interactionIcons.Length)
        {
            interactionIcons[index].gameObject.SetActive(true);
        }
    }

    private void setAllIconsInactive()
    {
        foreach (Image icon in interactionIcons)
        {
            icon.gameObject.SetActive(false);
        }
    }
}
