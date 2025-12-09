using UnityEngine;
using System.Collections;

public class BossController_Werewolf : MonoBehaviour
{
    public Transform player;
    public bool isAttacking = false;


    [Header("Movement")]
    public float moveSpeed = 2f;
    public float moveDistance = 2f;
    private Vector3 startPos;
    private float movementTime = 0f;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    public Transform healthBarFill;
    public GameObject healthBarRoot;
    public float smoothSpeed = 10f;

    private float fullBarWidth;
    private float fillHeight;
    private SpriteRenderer fillSprite;
    private DamageFlash flash;

    [Header("Boomerang Attack")]
    public float boomerangSpeed = 15f;
    public float boomerangWarningShakeDuration = 0.4f;
    public float boomerangWarningShakeAmount = 0.2f;
    public float boomerangCooldown = 4f;
    public float boomerangDistance = 12f;
    public GameObject boomerangHitbox;
    private bool canBoomerang = true;

    [Header("Motion Blur (Trail)")]
    public TrailRenderer trailEffect;
    public float trailTime = 0.25f;

    [Header("Phase 2 Settings")]
    public bool phase2 = false;
    public float phase2ShakeDuration = 1f;
    public float phase2ShakeAmount = 0.25f;

    // Stat buffs for phase 2
    public float phase2MoveSpeed = 3.5f;
    public float phase2BoomerangSpeed = 22f;
    public float phase2BoomerangCooldown = 2.5f;
    public float phase2TrailTime = 0.4f;

    [Header("Phase 2 Attack Buffs")]
    public float phase2BoomerangCooldownMultiplier = 0.6f;  // 40 percent faster
    public float phase2ClawCooldownMultiplier = 0.6f;       // 40 percent faster claws
    public float phase2MinAttackDistanceMultiplier = 1.2f;  // attacks further away



    void Start()
    {
        startPos = transform.position;
        currentHealth = maxHealth;

        fullBarWidth = healthBarFill.localScale.x;
        fillHeight = healthBarFill.localScale.y;

        fillSprite = healthBarFill.GetComponent<SpriteRenderer>();
        flash = GetComponent<DamageFlash>();

        if (trailEffect != null)
        {
            trailEffect.time = 0f;
        }
    }

    void Update()
    {
        MoveBoss();
        UpdateHealthBar();

        if (canBoomerang)
            TryBoomerangAttack();
    }

    // MOVEMENT
    void MoveBoss()
    {
        movementTime += Time.deltaTime * moveSpeed;
        float offsetY = Mathf.Sin(movementTime) * moveDistance;

        transform.position = new Vector3(
            transform.position.x,
            startPos.y + offsetY,
            transform.position.z
        );
    }

    // HEALTH
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        flash?.Flash();

        // PHASE 2 TRIGGER
        if (!phase2 && currentHealth <= maxHealth * 0.5f)
        {
            StartCoroutine(EnterPhase2());
        }

        if (currentHealth <= 0)
            Die();
    }


    void UpdateHealthBar()
    {
        float ratio = (float)currentHealth / maxHealth;
        float newX = Mathf.Lerp(
            healthBarFill.localScale.x,
            fullBarWidth * ratio,
            Time.deltaTime * smoothSpeed
        );

        healthBarFill.localScale = new Vector3(newX, fillHeight, 1f);
        fillSprite.color = (ratio <= 0.25f ? Color.red : Color.green);
    }

    void Die()
    {
        Destroy(gameObject);
    }

    // ATTACK HANDLING
    void TryBoomerangAttack()
    {
        float dist = Mathf.Abs(player.position.x - transform.position.x);

        if (dist < 10f)
        {
            StartCoroutine(BoomerangAttackRoutine());
        }
    }

    // FULL BOOMERANG ROUTINE
    IEnumerator BoomerangAttackRoutine()
    {
        if (!canBoomerang || isAttacking)
            yield break;

        isAttacking = true;
        canBoomerang = false;

        Vector3 originalPos = transform.position;

        // shake telegraph
        float elapsed = 0f;
        while (elapsed < boomerangWarningShakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * boomerangWarningShakeAmount;
            float offsetY = Random.Range(-1f, 1f) * boomerangWarningShakeAmount;

            transform.position = originalPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;

        // a little trail effect
        if (trailEffect != null)
            trailEffect.time = trailTime;

        float direction = (player.position.x < transform.position.x) ? -1f : 1f;
        Vector3 dashTarget = originalPos + new Vector3(direction * boomerangDistance, 0, 0);

        boomerangHitbox.SetActive(true);

        // Forward dash
        while (Vector3.Distance(transform.position, dashTarget) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                dashTarget,
                boomerangSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Return dash
        while (Vector3.Distance(transform.position, originalPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                originalPos,
                boomerangSpeed * Time.deltaTime
            );
            yield return null;
        }

        // shooting enabled when done with attack
        isAttacking = false;

        boomerangHitbox.SetActive(false);

        if (trailEffect != null)
            trailEffect.time = 0f;

        // no shooting during cooldown
        yield return new WaitForSeconds(boomerangCooldown);

        canBoomerang = true;
    }

    IEnumerator EnterPhase2()
    {
        phase2 = true;
        isAttacking = true; // prevent attacks during transformation

        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        // shake indicator for phase 2
        while (elapsed < phase2ShakeDuration)
        {
            float offsetX = Random.Range(-phase2ShakeAmount, phase2ShakeAmount);
            float offsetY = Random.Range(-phase2ShakeAmount, phase2ShakeAmount);

            transform.position = originalPos + new Vector3(offsetX, offsetY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;

        // move stats in phase 2
        moveSpeed = phase2MoveSpeed;
        boomerangSpeed = phase2BoomerangSpeed;
        boomerangCooldown *= phase2BoomerangCooldownMultiplier;

        // claw shooters adjustments
        ClawShooter[] claws = GetComponentsInChildren<ClawShooter>();
        foreach (var c in claws)
        {
            c.attackCooldown *= phase2ClawCooldownMultiplier;
            c.minAttackDistance *= phase2MinAttackDistanceMultiplier;
        }

        isAttacking = false; // allow attacks again
    }




}
