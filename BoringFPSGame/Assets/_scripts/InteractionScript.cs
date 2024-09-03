using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionScript : MonoBehaviour
{
    private Camera _Maincamera;
    [SerializeField] private float rayLength;

    private CapsuleCollider playerCollider;

    [Header("Interaction UI images")]
    public Image[] interactionIcons;

    private enum interactionIconType
    {
        JumpPad, 
        Chest,
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
                case "JumpPad":
                    SetIconsActive((int)interactionIconType.JumpPad);
                    break;

                case "Chest":
                    SetIconsActive((int)interactionIconType.Chest);
                    break;

                default:
                    setAllIconsInactive();
                    break;
            }
        }
        else
        {
            setAllIconsInactive();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;

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
        string tag = other.gameObject.tag;

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
