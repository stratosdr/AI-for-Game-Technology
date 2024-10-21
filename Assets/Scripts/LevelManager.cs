using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject youDiedPanel;   // Reference to You Died Panel
    public GameObject pauseMenuPanel; // Reference to Pause Menu Panel
    public GameObject pauseButton;    // Reference to the Pause Button
    public GameObject mainMenuPanel;  // Reference to Main Menu Panel
    public GameObject gameplayUI;     // Reference to Gameplay UI (optional)

    private CharacterMovement playerMovement;  // Reference to CharacterMovement
    private AnalyticsManager analyticsManager;

    void Start()
    {
        // Ensure the main menu is active at the start and gameplay UI is hidden
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (youDiedPanel != null) youDiedPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(false);

        // Find the player movement script
        playerMovement = FindObjectOfType<CharacterMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("CharacterMovement script not found!");
        }

        analyticsManager = FindObjectOfType<AnalyticsManager>();
        if (analyticsManager == null)
        {
            Debug.LogError("AnalyticsManager script not found!");
        }
    }

    // Method to start the game (hide main menu and show gameplay UI)
    public void StartGame()
    {
        ResetGame();  // Reset the game state each time the game starts fresh

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(true);

        analyticsManager.StartNewSession();

        Time.timeScale = 1f;  // Resume game time
        Debug.Log("Game Started");
    }

    // Method to reset the game state
    private void ResetGame()
    {
        if (playerMovement != null)
        {
            playerMovement.ResetPlayer();  // Reset player state
        }
        // Add any other game reset logic here, such as reinitializing enemies, score, etc.
        Debug.Log("Game reset to initial state");
    }

    // Method to show the You Died screen and hide the pause button
    public void ShowYouDiedScreen()
    {
        if (youDiedPanel != null)
        {
            youDiedPanel.SetActive(true);  // Display the You Died screen
        }

        if (pauseButton != null)
        {
            pauseButton.SetActive(false);  // Hide the Pause Button
        }

        Time.timeScale = 0f;  // Pause the game
        Debug.Log("Game Over: Player has died.");
    }

    // Method to pause the game and hide the pause button
    public void PauseGame()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);  // Display the Pause Menu
        }

        if (pauseButton != null)
        {
            pauseButton.SetActive(false);  // Hide the Pause Button
        }

        analyticsManager.RecordPause(); // Record pause in analytics
        Time.timeScale = 0f;  // Pause the game
        Debug.Log("Game Paused");
    }

    // Method to resume the game and show the pause button again
    public void ResumeGame()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);  // Hide the Pause Menu
        }

        if (pauseButton != null)
        {
            pauseButton.SetActive(true);  // Show the Pause Button
        }

        Time.timeScale = 1f;  // Resume the game
        Debug.Log("Game Resumed");
    }

    // Method to restart the game level
    public void RestartLevel()
    {
        ResetGame();  // Reset the game when restarting

        if (youDiedPanel != null)
        {
            youDiedPanel.SetActive(false);
        }

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

        if (pauseButton != null)
        {
            pauseButton.SetActive(true);
        }

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(true);
        }

        analyticsManager.RecordRestart(); // Record restart in analytics
        Time.timeScale = 1f;  // Ensure time is running normally
        Debug.Log("Game Restarted");
    }

    // Method to return to the main menu
    public void ReturnToMainMenu()
    {
        ResetGame();  // Reset the game when returning to the main menu

        analyticsManager.EndSession(false, false); // Start new session for analytics

        Time.timeScale = 1f;  // Resume time if paused

        if (youDiedPanel != null) youDiedPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(false);
        //if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        Debug.Log("Returned to Main Menu");
        SceneManager.LoadScene("MainMenuScene");
    }

    // Method to quit the game
    public void QuitGame()
    {
        Debug.Log("Quitting the game...");
        analyticsManager.EndSession(false, false);
        analyticsManager.SaveAnalytics(); // Save analytics on quit
        Application.Quit();  // Quit the game (doesn't work in editor)
    }
}
