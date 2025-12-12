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
    private bool isAttacking = false;

    private BossController_Werewolf wolf;

    void Start()
    {
        // assume this script is on a child of the werewolf
        wolf = GetComponentInParent<BossController_Werewolf>();
    }

    void Update()
    {
        if (player == null) return;

        // do not shoot while wolf is doing boomerang or phase transform
        if (wolf != null && wolf.isAttacking)
            return;

        if (!isAttacking)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance < minAttackDistance && canAttack)
                StartCoroutine(ClawShootRoutine());
        }
    }

    IEnumerator ClawShootRoutine()
    {
        canAttack = false;
        isAttacking = true;

        if (clawRenderer != null && attackClawSprite != null)
            clawRenderer.sprite = attackClawSprite;

        yield return new WaitForSeconds(attackWindup);

        float direction = (player.position.x < transform.position.x) ? -1f : 1f;

        GameObject proj = Instantiate(clawProjectilePrefab, firePoint.position, Quaternion.identity);
        var cp = proj.GetComponent<ClawProjectile>();
        if (cp != null)
            cp.direction = new Vector2(direction, 0);

        if (clawRenderer != null && idleClawSprite != null)
            clawRenderer.sprite = idleClawSprite;

        isAttacking = false;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
