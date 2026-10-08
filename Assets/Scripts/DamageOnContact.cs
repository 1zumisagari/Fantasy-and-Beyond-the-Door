using UnityEngine;

public class DamageOnContact : MonoBehaviour
{
    [SerializeField, Min(1)] private int damage = 1;

    // A solid enemy or spike can hurt the player on contact.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        DamagePlayer(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Staying here hurts again after the short safe period ends.
        DamagePlayer(collision.collider);
    }

    // Trigger colliders can hurt the player too.
    private void OnTriggerEnter2D(Collider2D other)
    {
        DamagePlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        DamagePlayer(other);
    }

    private void DamagePlayer(Collider2D other)
    {
        if (!isActiveAndEnabled || other.attachedRigidbody == null)
        {
            return;
        }

        // Only objects with PlayerHealth can lose player HP.
        PlayerHealth health = other.attachedRigidbody.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }
}
