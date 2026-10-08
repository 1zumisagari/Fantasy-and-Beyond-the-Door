using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class EnemyController : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField, Min(0f)] private float moveSpeed = 1.5f;
    [SerializeField, Min(0.1f)] private float patrolDistance = 1f;

    [Header("Health")]
    [SerializeField, Min(1)] private int maxHealth = 3;
    [SerializeField] private Color hitFlashColor = new Color(1f, 0.25f, 0.25f, 1f);
    [SerializeField, Min(0.01f)] private float hitFlashDuration = 0.15f;

    public bool StopForAttack { get; set; }
    public int CurrentHealth => health;

    private Rigidbody2D enemyBody;
    private SpriteRenderer enemySprite;
    private Color normalColor;
    private float startX;
    private float direction = 1f;
    private float flashUntil;
    private int health;
    private bool isDead;

    private void Awake()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        enemySprite = GetComponent<SpriteRenderer>();
        // Save the normal color so the hit flash can change back.
        normalColor = enemySprite.color;
        health = maxHealth;
        startX = transform.position.x;
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        // Stand still while getting ready to shoot.
        if (StopForAttack)
        {
            enemyBody.linearVelocity = new Vector2(0f, enemyBody.linearVelocity.y);
            return;
        }

        // Walk left and right around the starting position.
        // Keep the walking area on solid ground.
        if (enemyBody.position.x >= startX + patrolDistance)
        {
            direction = -1f;
        }
        else if (enemyBody.position.x <= startX - patrolDistance)
        {
            direction = 1f;
        }

        Vector2 velocity = enemyBody.linearVelocity;
        velocity.x = direction * moveSpeed;
        enemyBody.linearVelocity = velocity;
        enemySprite.flipX = direction < 0f;
    }

    private void Update()
    {
        // Show the hit color briefly, then go back to normal.
        enemySprite.color = Time.unscaledTime < flashUntil ? hitFlashColor : normalColor;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
        {
            return;
        }

        // Lose HP, but never let it go below zero.
        health = Mathf.Max(0, health - damage);
        flashUntil = Time.unscaledTime + hitFlashDuration;
        enemySprite.color = hitFlashColor;
        Debug.Log(name + " hit for " + damage + ". HP: " + health + "/" + maxHealth, this);

        if (health == 0)
        {
            isDead = true;
            // Hide the enemy right away when its HP runs out.
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
