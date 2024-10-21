using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPoint : MonoBehaviour
{
    private AnalyticsManager analyticsManager;

    void Start()
    {
        analyticsManager = FindObjectOfType<AnalyticsManager>(); // Get reference to AnalyticsManager
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            analyticsManager.EndSession(true, true); // End session on level completion
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}
