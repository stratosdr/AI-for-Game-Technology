using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        Time.timeScale = 1f;  // Unpause the game
        // Replace "GameScene" with the name of your game scene
        SceneManager.LoadScene("GameScene");
    }
}
