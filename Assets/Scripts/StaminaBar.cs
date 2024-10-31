using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Image staminaBarFill;  // This should reference the Image component in Fill mode.
    private RectTransform staminaBarRectTransform;
    

    void Start()
    {
        staminaBarRectTransform = staminaBarFill.GetComponent<RectTransform>();
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
                // Access the 'stamina' variables from CharacterMovement
                float playerstamina = playerScript.stamina;
                float playermstamina = playerScript.maxstamina;

                // Update the fill amount and color of the stamina bar
                float staminaPercentage = (float) playerstamina / playermstamina;

                // Adjust the scale to make it shrink from the center
                staminaBarRectTransform.localScale = new Vector3(staminaPercentage, 1f, 1f);
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
