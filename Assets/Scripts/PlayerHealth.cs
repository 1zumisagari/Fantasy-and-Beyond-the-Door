using UnityEngine;

[RequireComponent(typeof(PlayerController), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHealth = 3;
    [SerializeField, Min(0.01f)] private float invincibilityDuration = 1f;
    [SerializeField] private float fallDeathY = -10f;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private Color hitFlashColor = new Color(1f, 0.25f, 0.25f, 1f);
    [SerializeField, Min(0.01f)] private float hitFlashDuration = 0.15f;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;
    public bool IsDead { get; private set; }

    private PlayerController playerController;
    private PlayerAttack playerAttack;
    private Rigidbody2D playerBody;
    private SpriteRenderer playerSprite;
    private PlayerSounds playerSounds;
    private Color normalColor;
    private float safeUntil;
    private float flashUntil;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerAttack = GetComponent<PlayerAttack>();
        playerBody = GetComponent<Rigidbody2D>();
        // Keep checking contacts even when the player stands still.
        playerBody.sleepMode = RigidbodySleepMode2D.NeverSleep;
        playerSprite = GetComponent<SpriteRenderer>();
        playerSounds = GetComponent<PlayerSounds>();
        normalColor = playerSprite.color;
        // Start this level with full HP.
        CurrentHealth = maxHealth;
    }

    private void Start()
    {
        if (gameOverUI == null || !gameOverUI.isActiveAndEnabled)
        {
            Debug.LogError("PlayerHealth: Set Game Over UI on the active Canvas.", this);
        }
    }

    private void Update()
    {
        // Finish the flash even when the game is paused.
        playerSprite.color = Time.unscaledTime < flashUntil ? hitFlashColor : normalColor;
        if (IsDead)
        {
            return;
        }

        if (transform.position.y < fallDeathY)
        {
            // Falling below the level ends the game.
            CurrentHealth = 0;
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        // Ignore hits while dead or still safe from the last hit.
        if (!isActiveAndEnabled || IsDead || damage <= 0 || Time.time < safeUntil)
        {
            return;
        }

        // Lose HP and start a short safe period.
        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        safeUntil = Time.time + invincibilityDuration;
        flashUntil = Time.unscaledTime + hitFlashDuration;
        playerSprite.color = hitFlashColor;
        Debug.Log("Player HP: " + CurrentHealth + "/" + maxHealth, this);

        if (CurrentHealth == 0)
        {
            Die();
        }
        else if (playerSounds != null)
        {
            playerSounds.PlayHurt();
        }
    }

    private void Die()
    {
        if (IsDead)
        {
            return;
        }

        IsDead = true;
        if (playerSounds != null)
        {
            playerSounds.PlayDeath();
        }
        // Stop movement and attacks after death.
        playerController.enabled = false;
        if (playerAttack != null)
        {
            playerAttack.enabled = false;
        }
        playerBody.linearVelocity = Vector2.zero;
        playerBody.simulated = false;
        Debug.Log("Game Over", this);

        if (gameOverUI != null && gameOverUI.isActiveAndEnabled)
        {
            PlayerVisuals visuals = GetComponent<PlayerVisuals>();
            // Let the death poses finish before Game Over appears.
            float deathTime = visuals != null ? visuals.PlayDeath() : 0f;
            gameOverUI.ShowGameOver(deathTime);
        }
    }
}
