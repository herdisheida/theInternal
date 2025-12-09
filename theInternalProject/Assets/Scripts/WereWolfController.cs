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

        // --- Shake Telegraph ---
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

        // --- Motion Blur ---
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

        // BACK HOME → ALLOW SHOOTING AGAIN
        isAttacking = false;

        boomerangHitbox.SetActive(false);

        if (trailEffect != null)
            trailEffect.time = 0f;

        // Cooldown only blocks boomerang, NOT shooting now
        yield return new WaitForSeconds(boomerangCooldown);

        canBoomerang = true;
    }


}
