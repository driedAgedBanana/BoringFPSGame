using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CompassScript : MonoBehaviour
{
    public Transform player;
    public UnityEngine.UI.Image compassImage;

    void Update()
    {
        // Get the player's forward vector on the XZ plane
        Vector3 forward = player.transform.forward;

        // Calculate the angle between the player's forward direction and the world's north
        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

        compassImage.rectTransform.rotation = Quaternion.Euler(0, 0, -angle);
    }
}
