using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float Drag = 2.5f;
    public float DragThreshold = -5f;
    public float Smoothness = 5f;
    public Transform Parent;

    private Quaternion localRotation;
    // Start is called before the first frame update
    void Start()
    {
        localRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        float z = (Input.GetAxis("Mouse Y")) * Drag;
        float y = (Input.GetAxis("Mouse X")) * Drag;

        if (Drag >= 0)
        {
            y = (y > DragThreshold) ? DragThreshold : y;
            y = (y < -DragThreshold) ? -DragThreshold : y;

            z = (z > DragThreshold) ? DragThreshold : z;
            z = (z < - DragThreshold) ? -DragThreshold : z;
        }
        else
        {
            y = (y < DragThreshold) ? DragThreshold : y;
            y = (y >  -DragThreshold) ? -DragThreshold : y;

            z = (z < DragThreshold) ? -DragThreshold : z;
            z = (z > -DragThreshold) ? -DragThreshold : z;
        }

        Quaternion newRotation = Quaternion.Euler(localRotation.x, localRotation.y + y, Parent.localEulerAngles.z + localRotation.z + z);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, (Time.deltaTime * Smoothness));

        
    }
}
