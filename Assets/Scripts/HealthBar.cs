using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarFill;  // This should reference the Image component in Fill mode.
    private RectTransform healthBarRectTransform;
    
    private Color fullHealthColor = Color.green; // Color when health is full
    private Color zeroHealthColor = Color.red;   // Color when health is empty

    void Start()
    {
        healthBarRectTransform = healthBarFill.GetComponent<RectTransform>();
    }

    void FixedUpdate()
    {   
        GameObject player = GameObject.FindWithTag("Player");
        
        if (player != null)
        {
            // Get the CharacterMovement component attached to this player GameObject
            CharacterMovement playerScript = player.GetComponent<CharacterMovement>();
            if (playerScript != null)
            {
                // Access the 'health' variables from CharacterMovement
                int playerHealth = playerScript.currentHealth;
                int playermHealth = playerScript.maxHealth;

                // Update the fill amount and color of the health bar
                float healthPercentage = (float) playerHealth / playermHealth;
                healthBarFill.color = Color.Lerp(zeroHealthColor, fullHealthColor, healthPercentage);

                // Adjust the scale to make it shrink from the center
                healthBarRectTransform.localScale = new Vector3(healthPercentage, 1f, 1f);
            }
            else
            {
                Debug.LogError("CharacterMovement component not found on the player object!");
            }
        }
        else
        {
            Debug.LogError("Player GameObject with tag 'Player' not found!");
        }
    }
}
