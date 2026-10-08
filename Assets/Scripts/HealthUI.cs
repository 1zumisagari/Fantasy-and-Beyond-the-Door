using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TMP_Text healthText;

    private int displayedHealth = -1;

    private void Start()
    {
        if (playerHealth == null || healthText == null)
        {
            Debug.LogError("HealthUI: Assign Player Health and the HP Text on the Canvas.", this);
            enabled = false;
            return;
        }

        // Let mouse clicks pass through the HP text.
        healthText.raycastTarget = false;
        RefreshHealth();
    }

    private void Update()
    {
        // Change the text only when HP changes.
        if (playerHealth != null && displayedHealth != playerHealth.CurrentHealth)
        {
            RefreshHealth();
        }
    }

    private void RefreshHealth()
    {
        displayedHealth = playerHealth.CurrentHealth;
        // Show current HP and max HP together.
        healthText.text = "HP: " + displayedHealth + " / " + playerHealth.MaxHealth;
    }
}
