using UnityEngine;
using System.Collections;

public class BossController_Werewolf : MonoBehaviour
{
    public Transform player;
    [HideInInspector] public bool isAttacking = false;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float moveDistance = 2f;
    private Vector3 startPos;
    private float movementTime = 0f;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Health Bar")]
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

    public float phase2MoveSpeed = 3.5f;
    public float phase2BoomerangSpeed = 22f;
    public float phase2BoomerangCooldownMultiplier = 0.6f;
    public float phase2TrailTime = 0.4f;

    [Header("Phase 2 Attack Buffs")]
    public float phase2ClawCooldownMultiplier = 0.6f;
    public float phase2MinAttackDistanceMultiplier = 1.2f;

    void Start()
    {
        startPos = transform.position;
        currentHealth = maxHealth;

        fullBarWidth = healthBarFill.localScale.x;
        fillHeight = healthBarFill.localScale.y;

        fillSprite = healthBarFill.GetComponent<SpriteRenderer>();
        flash = GetComponent<DamageFlash>();

        if (trailEffect != null)
            trailEffect.time = 0f;
    }

    void Update()
    {
        MoveBoss();
        UpdateHealthBar();

        if (canBoomerang && !isAttacking)
            TryBoomerangAttack();
    }

    // MOVEMENT
    void MoveBoss()
    {
        if (isAttacking) return;

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

        // tiny hitstop for juice (optional singleton)
        // HitStop.instance?.StopTime(0.05f);

        // boss flash
        flash?.Flash();

        // shake health bar slightly
        if (healthBarRoot != null)
            StartCoroutine(ShakeHealthBar());

        // trigger phase 2 once below 50 percent, only if not already attacking
        if (!phase2 && !isAttacking && currentHealth <= maxHealth * 0.5f)
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

    IEnumerator ShakeHealthBar()
    {
        Vector3 original = healthBarRoot.transform.localPosition;
        float t = 0f;

        while (t < 0.15f)
        {
            float ox = Random.Range(-0.08f, 0.08f);
            float oy = Random.Range(-0.08f, 0.08f);
            healthBarRoot.transform.localPosition = original + new Vector3(ox, oy, 0);
            t += Time.deltaTime;
            yield return null;
        }

        healthBarRoot.transform.localPosition = original;
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

        // squash a bit before dash
        Vector3 originalScale = transform.localScale;
        transform.localScale = new Vector3(originalScale.x * 1.1f, originalScale.y * 0.9f, originalScale.z);

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
        transform.localScale = originalScale;

        // enable motion blur
        if (trailEffect != null)
            trailEffect.time = trailTime;

        float direction = (player.position.x < transform.position.x) ? -1f : 1f;
        Vector3 dashTarget = originalPos + new Vector3(direction * boomerangDistance, 0, 0);

        boomerangHitbox.SetActive(true);

        // camera shake on start of dash
        CameraShake.instance?.Shake(0.25f, 0.15f);

        // forward dash
        while (Vector3.Distance(transform.position, dashTarget) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                dashTarget,
                boomerangSpeed * Time.deltaTime
            );
            yield return null;
        }

        // return dash
        while (Vector3.Distance(transform.position, originalPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                originalPos,
                boomerangSpeed * Time.deltaTime
            );
            yield return null;
        }

        // in phase 2, do a quick second dash combo for more aggression
        if (phase2)
        {
            yield return new WaitForSeconds(0.15f);

            // recalc direction toward player again
            direction = (player.position.x < transform.position.x) ? -1f : 1f;
            dashTarget = transform.position + new Vector3(direction * (boomerangDistance * 0.8f), 0, 0);

            CameraShake.instance?.Shake(0.3f, 0.18f);

            // second forward dash
            while (Vector3.Distance(transform.position, dashTarget) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    dashTarget,
                    boomerangSpeed * Time.deltaTime
                );
                yield return null;
            }

            // return again
            while (Vector3.Distance(transform.position, originalPos) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    originalPos,
                    boomerangSpeed * Time.deltaTime
                );
                yield return null;
            }
        }

        boomerangHitbox.SetActive(false);

        // disable blur
        if (trailEffect != null)
            trailEffect.time = 0f;

        isAttacking = false;

        // cooldown only blocks next boomerang, not claws
        yield return new WaitForSeconds(boomerangCooldown);
        canBoomerang = true;
    }

    // PHASE 2 TRANSFORMATION
    IEnumerator EnterPhase2()
    {
        phase2 = true;
        isAttacking = true; // pause other attacks during transform

        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        // long dramatic shake
        while (elapsed < phase2ShakeDuration)
        {
            float offsetX = Random.Range(-phase2ShakeAmount, phase2ShakeAmount);
            float offsetY = Random.Range(-phase2ShakeAmount, phase2ShakeAmount);

            transform.position = originalPos + new Vector3(offsetX, offsetY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;

        // boost stats
        moveSpeed = phase2MoveSpeed;
        boomerangSpeed = phase2BoomerangSpeed;
        boomerangCooldown *= phase2BoomerangCooldownMultiplier;

        if (trailEffect != null)
            trailEffect.time = phase2TrailTime;

        // buff all claw shooters on the boss
        ClawShooter[] claws = GetComponentsInChildren<ClawShooter>();
        foreach (var c in claws)
        {
            c.attackCooldown *= phase2ClawCooldownMultiplier;
            c.minAttackDistance *= phase2MinAttackDistanceMultiplier;
        }

        // visual rage mode tint
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = new Color(1f, 0.3f, 0.3f);

        isAttacking = false;
    }
}
