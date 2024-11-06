using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private AnalyticsManager analyticsManager;

    void Start()
    {
        analyticsManager = FindObjectOfType<AnalyticsManager>();

        if (analyticsManager == null)
        {
            Debug.LogError("AnalyticsManager script not found!");
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;  // Unpause the game
        analyticsManager.StartNewSession();
        SceneManager.LoadScene("GameScene");
    }

    void Update()
    {
        // Check for 'T' key press to save analytics and reset player session
        if (Input.GetKeyDown(KeyCode.T))
        {
            analyticsManager.SaveAnalytics(true); // Save analytics
            Debug.Log("Analytics saved and new session started.");
        }
    }
}
