using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastViewer : MonoBehaviour
{
    public float weaponRange = 50f;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInParent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lineOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

        Debug.DrawRay(lineOrigin, cam.transform.forward * weaponRange, Color.red);
    }
}
