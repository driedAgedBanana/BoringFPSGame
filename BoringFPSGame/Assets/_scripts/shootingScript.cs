using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootingScript : MonoBehaviour
{
    //References
    public GunRecoil gunRecoil;

    public KeyCode shootKey = KeyCode.Mouse0;
    public Transform shootingPoint;
    public GameObject bullet;
    public float Power;
    [SerializeField] private float bulletTimeAlive = 5f;

    [SerializeField] private Transform mainCamera;

    private void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera").transform;
        gunRecoil = mainCamera.GetComponent<GunRecoil>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(shootKey))
        {
            GameObject bulletItSelf = Instantiate(bullet, shootingPoint.position, transform.rotation);
            bulletItSelf.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.right) * Power, ForceMode.VelocityChange);
            Destroy(bulletItSelf, bulletTimeAlive);
        }
    }
}
