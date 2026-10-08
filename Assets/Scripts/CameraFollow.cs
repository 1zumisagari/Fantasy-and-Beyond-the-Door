using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);
    [SerializeField, Min(0.01f)] private float smoothTime = 0.15f;

    // SmoothDamp uses this value to keep the camera movement smooth.
    private Vector3 followVelocity;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("CameraFollow: Set Player as Target in the Inspector.", this);
            enabled = false;
            return;
        }

        // Start the camera near the player.
        transform.position = target.position + offset;
    }

    private void LateUpdate()
    {
        // Skip camera movement if the player is missing or the game is paused.
        if (target == null || Time.deltaTime <= 0f)
        {
            return;
        }

        // Move the camera after the player moves.
        transform.position = Vector3.SmoothDamp(
            transform.position, target.position + offset,
            ref followVelocity, smoothTime);
    }
}
