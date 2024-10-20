using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarFill;  // This should reference the Image component in Fill mode.

    // Set the maximum health (this sets the fill to 1, i.e., full)
    public void SetMaxHealth(int health)
    {
        healthBarFill.fillAmount = 1f;  // Full bar at max health
    }

    // Update the health bar's fill amount based on the current health
    public void SetHealth(float currentHealth, float maxHealth)
    {
        Debug.Log($"Current Health: {currentHealth}, Max Health: {maxHealth}");

        healthBarFill.fillAmount = (float)currentHealth/ (float)maxHealth;
        Debug.Log("HealthBarFill: " + healthBarFill.fillAmount);
        Canvas.ForceUpdateCanvases();
    }
}
