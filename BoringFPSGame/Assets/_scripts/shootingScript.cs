using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootingScript : MonoBehaviour
{
    public KeyCode shootingKey = KeyCode.Mouse0;
    public int gunDamage = 1;
    public float fireRate = 0.25f;
    public float weaponRange = 50f;
    public float hitForce = 100f;
    public Transform gunEnd;

    private Camera cam;
    private WaitForSeconds shotDuration = new WaitForSeconds(0.75f);

    private LineRenderer shootingLine;
    private float nextFireRound;

    // Start is called before the first frame update
    void Start()
    {
        shootingLine = GetComponent<LineRenderer>();
        cam = GetComponentInParent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(shootingKey) && Time.time > nextFireRound)
        {
            nextFireRound = Time.time + fireRate;
            StartCoroutine(shootingMoment());

            Vector3 raycastOrigin = cam.ViewportToWorldPoint(new Vector3(.5f, .5f, 0.0f));

            RaycastHit hit;

            shootingLine.SetPosition(0, gunEnd.position);

            if (Physics.Raycast(raycastOrigin, cam.transform.forward, out hit, weaponRange))
            {
                shootingLine.SetPosition(1, hit.point);
            }
            else
            {
                shootingLine.SetPosition(1, cam.transform.forward * weaponRange);
            }
        }
    }

    private IEnumerator shootingMoment()
    {
        shootingLine.enabled = true;

        yield return shotDuration;

        shootingLine.enabled = false;
    }
}
