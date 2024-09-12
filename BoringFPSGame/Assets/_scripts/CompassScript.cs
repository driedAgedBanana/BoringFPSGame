using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassScript : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform
    public UnityEngine.UI.Image compassImage;  // Reference to the compass UI image

    void Update()
    {
        // Get the player's forward direction on the XZ plane (ignoring Y-axis to stay horizontal)
        Vector3 forward = player.transform.forward;

        // Calculate the angle between the player's forward vector and the north direction (Z-axis)
        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

        // Rotate the compass UI image based on the calculated angle
        compassImage.rectTransform.rotation = Quaternion.Euler(0, 0, -angle);
    }
}
