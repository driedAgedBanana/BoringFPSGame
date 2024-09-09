using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtonManagement : MonoBehaviour
{
    private PlayerMovementScript PMS;
    private GameOverManager gameOver;

    public void StartGame()
    {
        SceneManager.LoadScene("Playground 1");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game!");
    }

    //For game over HUD

    public void RestartGame()
    {
        SceneManager.LoadScene("Playground 1");
    }
}
