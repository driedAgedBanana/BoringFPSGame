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

    public void checkingOnPlayersHealth()
    {
        if (PMS != null && PMS.healthAmount <= 0)
        {
            TriggerGameOverPanel();
            Debug.Log("Player should die!");
        }
    }

   public void TriggerGameOverPanel()
    {
        gameOverHUD.enabled = true;
        playerHUD.enabled = false;

        Time.timeScale = 0;

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }
}
