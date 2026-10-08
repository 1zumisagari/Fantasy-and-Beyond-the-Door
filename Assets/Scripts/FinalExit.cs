using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FinalExit : MonoBehaviour
{
    [SerializeField] private GameClearUI gameClearUI;

    // Make the exit a trigger when this script is added.
    private void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void Start()
    {
        if (!GetComponent<BoxCollider2D>().isTrigger ||
            gameClearUI == null || !gameClearUI.isActiveAndEnabled)
        {
            Debug.LogError("FinalExit: Enable Is Trigger and assign an enabled GameClearUI on the Canvas.", this);
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActiveAndEnabled || other.attachedRigidbody == null)
        {
            return;
        }

        // Only the player can finish the game here.
        PlayerHealth player = other.attachedRigidbody.GetComponent<PlayerHealth>();
        if (player != null)
        {
            gameClearUI.ShowGameClear(player);
        }
    }
}
