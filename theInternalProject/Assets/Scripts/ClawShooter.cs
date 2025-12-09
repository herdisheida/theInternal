using UnityEngine;
using System.Collections;


public class ClawShooter : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform firePoint;
    public SpriteRenderer clawRenderer;
    public GameObject clawProjectilePrefab;

    [Header("Sprites")]
    public Sprite idleClawSprite;
    public Sprite attackClawSprite;

    [Header("Settings")]
    public float attackCooldown = 1.2f;
    public float attackWindup = 0.1f;
    public float minAttackDistance = 12f;

    private bool canAttack = true;
    private bool isAttacking = false;   // blocks other attacks
    private BossController_Werewolf wolf;


    void Update()
    {
        if (!isAttacking)
        {
            if (player == null) return;

            float distance = Vector2.Distance(transform.position, player.position);

            // Only attack when close enough and ready
            if (distance < minAttackDistance && canAttack)
            {
                StartCoroutine(ClawShootRoutine());
            }
        }
    }

    IEnumerator ClawShootRoutine()
    {
        canAttack = false;

        // Change claw sprite to attack
        if (clawRenderer != null && attackClawSprite != null)
            clawRenderer.sprite = attackClawSprite;

        yield return new WaitForSeconds(attackWindup);

        // Determine direction
        float direction = (player.position.x < transform.position.x) ? -1f : 1f;

        // Spawn projectile
        GameObject proj = Instantiate(clawProjectilePrefab, firePoint.position, Quaternion.identity);
        var cp = proj.GetComponent<ClawProjectile>();
        if (cp != null)
            cp.direction = new Vector2(direction, 0);

        // Reset sprite
        if (clawRenderer != null && idleClawSprite != null)
            clawRenderer.sprite = idleClawSprite;

        // Cooldown
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
