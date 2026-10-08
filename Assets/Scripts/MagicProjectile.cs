using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MagicProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float hitRadius = 0.12f;
    [SerializeField] private int damage = 1;
    private Vector2 direction;
    private Transform owner;
    private bool fromPlayer;

    public void Launch(Vector2 travelDirection, Transform shooter, bool playerShot)
    {
        // Keep the direction the same length so shots move at the same speed.
        direction = travelDirection.normalized;
        owner = shooter;
        fromPlayer = playerShot;
        // Remove the shot after a few seconds if it hits nothing.
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (owner == null) { Destroy(gameObject); return; }
        float distance = speed * Time.fixedDeltaTime;
        // Shot cannot pass through a thin platform.
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, hitRadius, direction, distance);
        foreach (RaycastHit2D hit in hits)
        {
            // Skip the character that fired this shot.
            if (hit.collider.transform.IsChildOf(owner)) continue;
            EnemyController enemy = hit.collider.GetComponentInParent<EnemyController>();
            PlayerHealth player = hit.collider.GetComponentInParent<PlayerHealth>();
            // Player shots hurt enemies.
            if (fromPlayer && enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
            // Enemy shots hurt the player.
            if (!fromPlayer && player != null)
            {
                player.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
            // Ground and platforms stop the shot.
            if (!hit.collider.isTrigger && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Destroy(gameObject);
                return;
            }
        }
        // Move the shot forward for this physics step.
        transform.position += (Vector3)(direction * distance);
    }
}
