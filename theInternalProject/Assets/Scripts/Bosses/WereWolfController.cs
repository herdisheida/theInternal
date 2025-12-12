using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


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

    [Header("Motion Blur")]
    public TrailRenderer trailEffect;
    public float trailTime = 0.25f;

    [Header("Afterimage")]
    public AfterImageGenerator afterImages;

    [Header("Phase 2 Settings")]
    public bool phase2 = false;
    public float phase2ShakeDuration = 1f;
    public float phase2ShakeAmount = 0.25f;

    public float phase2MoveSpeed = 3.5f;
    public float phase2BoomerangSpeed = 22f;
    public float phase2BoomerangCooldownMultiplier = 0.6f;
    public float phase2TrailTime = 0.4f;

    public float phase2ClawCooldownMultiplier = 0.6f;
    public float phase2MinAttackDistanceMultiplier = 1.2f;


    [Header("Death Animation")]
    public float deathShakeDuration = 1f;
    public float deathShakeMagnitude = 0.12f;
    public float deathFallSpeed = 6f;
    public float deathFallRotationSpeed = 180f;   // degrees per second
    public float deathFallDistance = 6f;          // how far down he falls
    public string deathNextScene = "AnalysisScreen";

    private bool isDying = false;
    private Coroutine BoomerangAttack; 


    void Start()
    {
        AudioManager.instance?.PlayWerewolfBossBattleMusic();

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

        // Only try Boomerang if allowed and not currently doing another attack
        if (canBoomerang && !isAttacking && !isDying)
            TryBoomerangAttack();
    }

    // ---------------- MOVEMENT ----------------
    void MoveBoss()
    {
        if (isAttacking || isDying) return;

        movementTime += Time.deltaTime * moveSpeed;
        float offsetY = Mathf.Sin(movementTime) * moveDistance;

        transform.position = new Vector3(
            transform.position.x,
            startPos.y + offsetY,
            transform.position.z
        );
    }

    // ---------------- HEALTH ----------------
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        flash?.Flash();

        // health bar shake
        if (healthBarRoot != null)
            StartCoroutine(ShakeHealthBar());

        // enter phase 2 once, below 50 percent, and ONLY if not in attack animation
        if (!phase2 && !isAttacking && currentHealth <= maxHealth * 0.5f)
            StartCoroutine(EnterPhase2());


        if (1 <= currentHealth && currentHealth <= 3) StopAllCoroutines();

        if (currentHealth <= 0 && !isDying)
        {
            isDying = true;
            GameManager.instance?.MarkPatientSaved();
            StartCoroutine(Die());
        }
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

    IEnumerator Die()
    {
        // stop all ongoing attacks/movement
        if (BoomerangAttack != null) 
        {
            StopCoroutine(BoomerangAttack);
            BoomerangAttack = null;
        }

        // sound effect
        AudioManager.instance?.WerewolfHowling();

        // small camera shake when he dies
        CameraShake.instance?.Shake(0.4f, 0.2f);

        Vector3 originalPos = transform.position;

        // ---- SHAKE IN PLACE ----
        float elapsed = 0f;
        while (elapsed < deathShakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * deathShakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * deathShakeMagnitude;

            transform.position = originalPos + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // snap back
        transform.position = originalPos;

        // ---- FALL OFF SCREEN ----
        Vector3 targetPos = originalPos + Vector3.down * deathFallDistance;

        while (transform.position.y > targetPos.y)
        {
            // move down
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                deathFallSpeed * Time.deltaTime
            );

            // spin while falling
            transform.Rotate(0f, 0f, deathFallRotationSpeed * Time.deltaTime);

            yield return null;
        }


        // change scene
        yield return new WaitForSeconds(2f);
        StopAllCoroutines();
        SceneManager.LoadScene(deathNextScene);
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

    // ---------------- ATTACK LOGIC ----------------
    void TryBoomerangAttack()
    {
        if (isDying) return;

        float dist = Mathf.Abs(player.position.x - transform.position.x);

        if (dist < 10f)
            StartCoroutine(BoomerangAttackRoutine());
    }

    // ---------------- BOOMERANG ATTACK ----------------
    IEnumerator BoomerangAttackRoutine()
    {
        AudioManager.instance?.WerewolfGrowl();
        AudioManager.instance?.WerewolfChomp();

        // Check BEFORE enabling blur or effects
        if (!canBoomerang || isAttacking || isDying)
            yield break;

        isAttacking = true;
        canBoomerang = false;

        // Now safe to activate effects
        EnableDashBlur();
        afterImages?.StartAfterImages();

        Vector3 originalPos = transform.position;
        Vector3 originalScale = transform.localScale;

        // squash/stretch before dash
        transform.localScale = new Vector3(originalScale.x * 1.1f, originalScale.y * 0.9f, originalScale.z);

        // Telegraph shake
        float elapsed = 0f;
        while (elapsed < boomerangWarningShakeDuration)
        {
            float ox = Random.Range(-boomerangWarningShakeAmount, boomerangWarningShakeAmount);
            float oy = Random.Range(-boomerangWarningShakeAmount, boomerangWarningShakeAmount);

            transform.position = originalPos + new Vector3(ox, oy, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // reset
        transform.position = originalPos;
        transform.localScale = originalScale;

        float direction = (player.position.x < transform.position.x) ? -1f : 1f;
        Vector3 dashTarget = originalPos + new Vector3(direction * boomerangDistance, 0, 0);

        boomerangHitbox.SetActive(true);
        CameraShake.instance?.Shake(0.25f, 0.15f);

        // first dash
        while (Vector3.Distance(transform.position, dashTarget) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                dashTarget,
                boomerangSpeed * Time.deltaTime
            );
            yield return null;
        }

        // return
        while (Vector3.Distance(transform.position, originalPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                originalPos,
                boomerangSpeed * Time.deltaTime
            );
            yield return null;
        }

        // PHASE 2 BONUS DASH
        if (phase2)
        {
            yield return new WaitForSeconds(0.15f);

            direction = (player.position.x < transform.position.x) ? -1f : 1f;
            dashTarget = transform.position + new Vector3(direction * (boomerangDistance * 0.8f), 0, 0);

            CameraShake.instance?.Shake(0.3f, 0.18f);

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

        DisableDashBlur();   
        afterImages?.StopAfterImages();

        isAttacking = false;

        yield return new WaitForSeconds(boomerangCooldown);
        canBoomerang = true;
    }

    // ---------------- PHASE 2 ----------------
    IEnumerator EnterPhase2()
    {
        AudioManager.instance?.WerewolfGrowl();

        phase2 = true;
        isAttacking = true;

        Vector3 originalPos = transform.position;
        float t = 0f;

        // shaking transition
        while (t < phase2ShakeDuration)
        {
            float ox = Random.Range(-phase2ShakeAmount, phase2ShakeAmount);
            float oy = Random.Range(-phase2ShakeAmount, phase2ShakeAmount);

            transform.position = originalPos + new Vector3(ox, oy, 0);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;

        // upgrade stats
        moveSpeed = phase2MoveSpeed;
        boomerangSpeed = phase2BoomerangSpeed;
        boomerangCooldown *= phase2BoomerangCooldownMultiplier;

        // trail becomes stronger in phase 2
        if (trailEffect != null)
            trailEffect.time = phase2TrailTime;

        // buff all claws
        ClawShooter[] claws = GetComponentsInChildren<ClawShooter>();
        foreach (var claw in claws)
        {
            claw.attackCooldown *= phase2ClawCooldownMultiplier;
            claw.minAttackDistance *= phase2MinAttackDistanceMultiplier;
        }

        // red tint for rage mode
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = new Color(1f, 0.3f, 0.3f);

        isAttacking = false;
    }

    // ---------------- BLUR ----------------
    void EnableDashBlur()
    {
        if (trailEffect != null)
            trailEffect.time = trailTime;
    }

    void DisableDashBlur()
    {
        if (trailEffect == null) return;

        // In phase 2, keep a small blur always on
        if (phase2)
            trailEffect.time = phase2TrailTime;
        else
            trailEffect.time = 0f;
    }
}
