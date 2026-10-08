using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField, Min(0f)] private float jumpForce = 5.8f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField, Min(0.01f)] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    public bool IsGrounded { get; private set; }

    public bool FacingRight { get; private set; } = true;

    private SpriteRenderer playerSprite;
    private Rigidbody2D playerBody;
    private PlayerSounds playerSounds;
    private float moveInput;
    private bool jumpPressed;

    private void Awake()
    {
        // Get the components on this Player object.
        playerBody = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerSounds = GetComponent<PlayerSounds>();

        if (groundCheck == null || groundLayer.value == 0)
        {
            Debug.LogError("PlayerController: Set Ground Check and Ground Layer in the Inspector.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        // Clear the last input before reading the keys.
        moveInput = 0f;
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        // A moves left. D moves right. Both together cancel out.
        if (keyboard.aKey.isPressed)
        {
            moveInput -= 1f;
        }
        if (keyboard.dKey.isPressed)
        {
            moveInput += 1f;
        }
        // Face the way the player is moving.
        if (moveInput != 0f)
        {
            FacingRight = moveInput > 0f;
            if (playerSprite != null)
            {
                playerSprite.flipX = !FacingRight;
            }
        }

        // Remember the jump press for the next physics step.
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            jumpPressed = true;
        }
    }

    private void FixedUpdate()
    {
        // Check the small circle below the feet for solid ground.
        Collider2D groundHit = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundLayer);
        IsGrounded = groundHit != null && !groundHit.isTrigger && playerBody.linearVelocity.y <= 0.01f;

        // Change horizontal speed and keep the current falling speed.
        Vector2 velocity = playerBody.linearVelocity;
        velocity.x = moveInput * moveSpeed;

        // Jump only when on the ground and not moving up.
        if (jumpPressed && IsGrounded && velocity.y <= 0.01f)
        {
            velocity.y = jumpForce;
            IsGrounded = false;
            if (playerSounds != null)
            {
                playerSounds.PlayJump();
            }
        }

        playerBody.linearVelocity = velocity;
        // A new jump needs a new Space press.
        jumpPressed = false;
    }

    private void OnDisable()
    {
        moveInput = 0f;
        jumpPressed = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        // Draw the ground-check circle in the Editor.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
