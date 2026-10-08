using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerRangedAttack : MonoBehaviour
{
    [SerializeField] private MagicProjectile projectilePrefab;
    [SerializeField] private float cooldown = 0.65f;
    private PlayerController movement;
    private PlayerHealth health;
    private float nextShot;

    private void Awake()
    {
        movement = GetComponent<PlayerController>();
        health = GetComponent<PlayerHealth>();
    }

    // This script is only added to the Level2 player.
    private void LateUpdate()
    {
        if (!movement.isActiveAndEnabled || health == null || health.IsDead ||
            Time.timeScale <= 0f || Keyboard.current == null || projectilePrefab == null) return;
        // K fires one shot when the cooldown is over.
        if (Keyboard.current.kKey.wasPressedThisFrame && Time.time >= nextShot)
        {
            nextShot = Time.time + cooldown;
            // Shoot in the direction the player is facing.
            Vector2 direction = movement.FacingRight ? Vector2.right : Vector2.left;
            // Spawn the shot just in front of the player.
            Vector3 origin = transform.position + (Vector3)(direction * 0.75f);
            MagicProjectile shot = Instantiate(projectilePrefab, origin, Quaternion.identity);
            shot.Launch(direction, transform, true);
            PlayerSounds sounds = GetComponent<PlayerSounds>();
            if (sounds != null) sounds.PlayAttack();
        }
    }
}
