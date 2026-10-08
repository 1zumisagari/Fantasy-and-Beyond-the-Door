using UnityEngine;

[RequireComponent(typeof(EnemyController), typeof(SpriteRenderer))]
public class EnemyRangedAttack : MonoBehaviour
{
    [SerializeField] private MagicProjectile projectilePrefab;
    [SerializeField] private Sprite[] attackFrames;
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float shotInterval = 2.4f;
    [SerializeField] private float windup = 0.45f;
    private PlayerHealth player;
    private EnemyController enemy;
    private SpriteRenderer picture;
    private Sprite idle;
    private float nextShot;
    private float attackStarted;
    private bool preparing;
    private float facing;

    private void Start()
    {
        GameObject obj = GameObject.Find("Player");
        player = obj != null ? obj.GetComponent<PlayerHealth>() : null;
        enemy = GetComponent<EnemyController>();
        picture = GetComponent<SpriteRenderer>();
        idle = picture.sprite;
        // Give the player a moment before the first shot.
        nextShot = Time.time + 1f;
    }

    private void LateUpdate()
    {
        if (Time.timeScale <= 0f || player == null || player.IsDead || !player.isActiveAndEnabled) return;
        // Play the attack poses before firing.
        if (preparing)
        {
            picture.flipX = facing < 0f;
            float elapsed = Time.time - attackStarted;
            if (attackFrames != null && attackFrames.Length > 0)
                picture.sprite = attackFrames[Mathf.Min((int)(elapsed / windup * attackFrames.Length), attackFrames.Length - 1)];
            if (elapsed >= windup)
            {
                // Create a shot in front of the enemy.
                MagicProjectile shot = Instantiate(projectilePrefab,
                    transform.position + new Vector3(facing * 0.7f, 0f, 0f), Quaternion.identity);
                shot.Launch(new Vector2(facing, 0f), transform, false);
                preparing = false;
                enemy.StopForAttack = false;
                picture.sprite = idle;
                // Wait a little before shooting again.
                nextShot = Time.time + shotInterval;
            }
            return;
        }
        // Check how far away the player is.
        Vector2 difference = player.transform.position - transform.position;
        if (projectilePrefab != null && Time.time >= nextShot &&
            Mathf.Abs(difference.x) < detectionRange && Mathf.Abs(difference.y) < 1.2f)
        {
            // Do not shoot when ground or a platform blocks the way.
            RaycastHit2D cover = Physics2D.Linecast(transform.position, player.transform.position,
                LayerMask.GetMask("Ground"));
            if (cover.collider != null) return;
            facing = difference.x >= 0f ? 1f : -1f;
            preparing = true;
            enemy.StopForAttack = true;
            attackStarted = Time.time;
        }
    }
}
