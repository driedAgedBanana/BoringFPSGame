using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionScript : MonoBehaviour
{
    private Camera _Maincamera;
    [SerializeField] private float rayLength;

    [Header("Interaction UI images")]
    public Image teleportationIcon;

    // Start is called before the first frame update
    void Start()
    {
        teleportationIcon.gameObject.SetActive(false);

        Camera[] mainCam = GetComponentsInChildren<Camera>();
        foreach (Camera cam in mainCam)
        {
            if (cam.CompareTag("MainCamera"))
            {
                _Maincamera = cam;
            }
        }
    }

    // Update is called once per frame
    void Update()
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
            //Transform objectHit = hit.transform; //pure for the debug
            //Debug.Log(objectHit.transform.name);

            if (hit.collider.tag == "Teleport")
            {
                teleportationIcon.gameObject.SetActive(true);
            }
            else
            {
                teleportationIcon.gameObject.SetActive(false);
            }
        }
    }
}
