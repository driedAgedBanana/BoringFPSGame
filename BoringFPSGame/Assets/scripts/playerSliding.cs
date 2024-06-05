using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerController playerController;

    [Header("Slider")]
    public float maxSlideTime;
    public float slideForce;
    [SerializeField] private float slideTimer;

    public float slideYScale;
    [SerializeField] private float startYScale;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    private bool isSliding;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();

        startYScale = playerObj.localScale.y;
    }
}
