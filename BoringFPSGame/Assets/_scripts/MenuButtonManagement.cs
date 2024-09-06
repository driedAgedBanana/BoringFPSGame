using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtonManagement : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Playground 1");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game!");
    }
}
