using UnityEngine;

[RequireComponent(typeof(PlayerController), typeof(SpriteRenderer))]
public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite[] walkFrames;
    [SerializeField] private Sprite[] jumpFrames;
    [SerializeField] private Sprite[] deathFrames;
    [SerializeField, Min(0.01f)] private float jumpFrameDuration = 0.12f;
    [SerializeField, Min(0.01f)] private float walkFrameDuration = 0.1f;
    [SerializeField, Min(0.01f)] private float deathFrameDuration = 0.14f;
    [SerializeField] private GameObject weaponRoot;

    private PlayerController controller;
    private SpriteRenderer picture;
    private Rigidbody2D body;
    private bool wasMoving;
    private float walkStartTime;
    private bool wasInAir;
    private bool isDead;
    private float jumpStartTime;
    private float deathStartTime;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        picture = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        if (idleSprite != null) picture.sprite = idleSprite;
    }

    private void LateUpdate()
    {
        if (isDead)
        {
            // Play the death frames while the game is paused.
            ShowFrame(deathFrames, Time.unscaledTime - deathStartTime, deathFrameDuration);
            return;
        }
        if (!controller.isActiveAndEnabled || Time.timeScale <= 0f) return;

        // Use jump poses whenever the feet are off the ground.
        bool inAir = !controller.IsGrounded;
        if (inAir && !wasInAir) jumpStartTime = Time.time;
        if (inAir)
        {
            ShowFrame(jumpFrames, Time.time - jumpStartTime, jumpFrameDuration);
            wasMoving = false;
        }
        else if (Mathf.Abs(body.linearVelocity.x) > 0.1f && walkFrames != null && walkFrames.Length > 0)
        {
            // Start the walk frames from the beginning when moving starts.
            if (!wasMoving) walkStartTime = Time.time;
            // The remainder (%) loops back to the first walk frame.
            int frame = Mathf.FloorToInt((Time.time - walkStartTime) / walkFrameDuration) % walkFrames.Length;
            if (walkFrames[frame] != null) picture.sprite = walkFrames[frame];
            wasMoving = true;
        }
        else
        {
            // Show the standing pose when the player stops.
            if (idleSprite != null) picture.sprite = idleSprite;
            wasMoving = false;
        }
        wasInAir = inAir;
    }

    public float PlayDeath()
    {
        if (isDead) return 0f;
        isDead = true;
        deathStartTime = Time.unscaledTime;
        // Hide the weapon during the death animation.
        if (weaponRoot != null) weaponRoot.SetActive(false);
        ShowFrame(deathFrames, 0f, deathFrameDuration);
        return deathFrames != null ? deathFrames.Length * deathFrameDuration : 0f;
    }

    private void ShowFrame(Sprite[] frames, float elapsed, float frameDuration)
    {
        if (frames == null || frames.Length == 0) return;
        // Stop on the last frame.
        int frame = Mathf.Min(Mathf.FloorToInt(elapsed / frameDuration), frames.Length - 1);
        if (frames[frame] != null) picture.sprite = frames[frame];
    }
}
