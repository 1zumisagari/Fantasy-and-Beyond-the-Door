using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class DoorTransition : MonoBehaviour
{
    [SerializeField] private string destinationSceneName = "Level1";

    private bool isLoading;

    private void Reset()
    {
        // Reset runs when this component is first added in the Editor.
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void Awake()
    {
        if (!GetComponent<BoxCollider2D>().isTrigger)
        {
            Debug.LogError("DoorTransition: Enable Is Trigger on the door's BoxCollider2D.", this);
            enabled = false;
            return;
        }

        // Check that the next scene is ready to load.
        if (string.IsNullOrWhiteSpace(destinationSceneName) ||
            !Application.CanStreamedLevelBeLoaded(destinationSceneName))
        {
            Debug.LogError("DoorTransition: Add the destination scene to the active Build Profiles Scene List and check Destination Scene Name: " + destinationSceneName, this);
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled || isLoading || other.attachedRigidbody == null)
        {
            return;
        }

        // Only the player can use this door.
        PlayerController player = other.attachedRigidbody.GetComponent<PlayerController>();
        if (player == null || !player.isActiveAndEnabled)
        {
            return;
        }

        // Load the next scene only once.
        isLoading = true;
        // Leave this scene and open the next one.
        SceneManager.LoadScene(destinationSceneName, LoadSceneMode.Single);
    }
}
