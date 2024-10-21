using UnityEngine;

public class EndLevel : MonoBehaviour 
{
    private AnalyticsManager analyticsManager;

    void Start()
    {
        analyticsManager = FindObjectOfType<AnalyticsManager>(); // Get reference to AnalyticsManager
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player")) 
        {
            analyticsManager.EndSession(true, true); // End session on level completion
            ShowGameEndScreen();
        }
    }

    void ShowGameEndScreen() 
    {
        Debug.Log("Player reached the end! Game over.");
        // GameManager.Instance.ShowEndScreen();
    }
}
