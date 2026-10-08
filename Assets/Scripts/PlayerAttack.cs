using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Weapon References")]
    [SerializeField] private Transform weaponRoot;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private SpriteRenderer weaponSprite;

    [Header("Attack Effect")]
    [SerializeField] private SpriteRenderer attackEffect;
    [SerializeField, Min(0.01f)] private float effectDuration = 0.18f;

    [Header("Attack")]
    [SerializeField, Min(0.01f)] private float attackRadius = 0.5f;
    [SerializeField, Min(1)] private int attackDamage = 1;
    [SerializeField, Min(0.01f)] private float attackCooldown = 0.4f;

    private PlayerController player;
    private PlayerSounds playerSounds;
    private Vector3 weaponScale;
    private Color weaponColor;
    private float nextAttackTime;
    private float flashEndTime;
    private float effectStartTime;
    private Vector3 effectScale;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        playerSounds = GetComponent<PlayerSounds>();
        if (weaponRoot == null || attackPoint == null || weaponSprite == null ||
            weaponRoot == transform || !weaponRoot.IsChildOf(transform) ||
            !attackPoint.IsChildOf(weaponRoot) || !weaponSprite.transform.IsChildOf(weaponRoot))
        {
            Debug.LogError("PlayerAttack: Assign a child WeaponRoot containing AttackPoint and the Weapon SpriteRenderer.", this);
            enabled = false;
            return;
        }

        // Remember the weapon size and color.
        weaponScale = weaponRoot.localScale;
        weaponColor = weaponSprite.color;
        if (attackEffect != null)
        {
            effectScale = attackEffect.transform.localScale;
            attackEffect.enabled = false;
        }
    }

    private void LateUpdate()
    {
        // Turn the weapon and its attack point with the player.
        Vector3 scale = weaponScale;
        scale.x = Mathf.Abs(scale.x) * (player.FacingRight ? 1f : -1f);
        weaponRoot.localScale = scale;
        weaponSprite.color = Time.time < flashEndTime ? Color.white : weaponColor;
        UpdateAttackEffect();

        // J attacks only after the cooldown is over.
        Keyboard keyboard = Keyboard.current;
        if (player.isActiveAndEnabled && keyboard != null &&
            keyboard.jKey.wasPressedThisFrame && Time.time >= nextAttackTime)
        {
            Attack();
        }
    }

    private void Attack()
    {
        // Set the time when the next attack is allowed.
        nextAttackTime = Time.time + attackCooldown;
        flashEndTime = Time.time + 0.12f;
        weaponSprite.color = Color.white;
        if (attackEffect != null)
        {
            effectStartTime = Time.time;
            attackEffect.color = Color.white;
            attackEffect.transform.localScale = effectScale * 0.85f;
            attackEffect.enabled = true;
        }
        if (playerSounds != null)
        {
            playerSounds.PlayAttack();
        }

        // Find enemies inside the attack circle.
        // Each enemy loses HP only once per swing.
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius);
        HashSet<EnemyController> damagedEnemies = new HashSet<EnemyController>();
        foreach (Collider2D hit in hits)
        {
            EnemyController enemy = hit.GetComponentInParent<EnemyController>();
            if (enemy != null && enemy.isActiveAndEnabled && damagedEnemies.Add(enemy))
            {
                enemy.TakeDamage(attackDamage);
            }
        }
    }

    private void UpdateAttackEffect()
    {
        if (attackEffect == null || !attackEffect.enabled)
        {
            return;
        }

        // Work out how far the slash effect has played.
        float progress = (Time.time - effectStartTime) / effectDuration;
        if (progress >= 1f)
        {
            attackEffect.enabled = false;
            return;
        }

        // Make the slash grow a little and fade out.
        attackEffect.transform.localScale = effectScale * Mathf.Lerp(0.85f, 1f, progress);
        attackEffect.color = new Color(1f, 1f, 1f, 1f - progress);
    }

    private void OnDisable()
    {
        if (attackEffect != null)
        {
            attackEffect.enabled = false;
        }
        if (weaponSprite != null)
        {
            weaponSprite.color = weaponColor;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
