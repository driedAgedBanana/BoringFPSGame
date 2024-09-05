using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public Canvas gameOverHUD;
    public Canvas playerHUD;

    private PlayerMovementScript PMS;

    // Start is called before the first frame update
    void Start()
    {
        PMS = GetComponent<PlayerMovementScript>();

        if (PMS == null )
        {
            Debug.LogError("Boy you forgot to assign the playerMovementScript into this script!");
        }

        gameOverHUD.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        checkingOnPlayersHealth();
    }

    private void checkingOnPlayersHealth()
    {
        if (PMS != null && PMS.healthAmount <= 0)
        {
            triggerGameOverPanel();
            Debug.Log("Player should die!");
        }
    }

    private void triggerGameOverPanel()
    {
        playerHUD.enabled = false;
        gameOverHUD.enabled = true;

        PMS.enabled = false;
        Destroy(PMS.gameObject);

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }
}
