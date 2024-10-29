using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody rb;
    private LevelManager levelManager;
    private AnalyticsManager analyticsManager;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Find the LevelManager in the scene
        levelManager = FindObjectOfType<LevelManager>();
        analyticsManager = FindObjectOfType<AnalyticsManager>();

        if (levelManager == null)
        {
            Debug.LogError("LevelManager not found in the scene!");
        }
        if (analyticsManager == null)
        {
            Debug.LogError("AnalyticsManager not found in the scene!");
        }
        //Destroy(gameObject, 5f);
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CharacterMovement playerMovement = collision.gameObject.GetComponent<CharacterMovement>();
            if (playerMovement != null)
            {

                playerMovement.TakeDamage(1);
                if (analyticsManager != null) {
                    analyticsManager.RecordBulletDamage(1);

                }
                if (playerMovement.currentHealth <= 0 && levelManager != null)
                {
                    levelManager.ShowYouDiedScreen();  // Directly call ShowYouDiedScreen instead of using SendMessage
                }


            }
        } else{
            analyticsManager.RecordBulletMissed();
        }
        Destroy(gameObject);

    }


}
