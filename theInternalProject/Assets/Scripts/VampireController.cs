using UnityEngine;
using System.Collections;

public class BossController_Vampire : MonoBehaviour
{
    public Transform player;
    [HideInInspector] public bool isAttacking = false;

    // movement settings
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float hoverHeight = 2f;
    private float moveTime = 0f;
    private Vector3 startPos;

    // health system
    [Header("Health")]
    public int maxHealth = 120;
    private int currentHealth;

    public Transform healthBarFill;
    public GameObject healthBarRoot;
    public float smoothSpeed = 10f;

    private float fullBarWidth;
    private float fillHeight;
    private SpriteRenderer fillSprite;
    private DamageFlash flash;

    // wind attack both phases
    [Header("Wind Slash (Phase 1 + 2)")]
    public GameObject windSlashPrefab;
    public Transform windShootPoint;
    public float windSlashCooldown = 2f;
    public float windSlashSpeed = 8f;
    public float windMinDist = 12f;

    private bool canWindSlash = true;

    // the life drain in phase 2
    [Header("Life Drain Aura (Phase 2)")]
    public GameObject lifeDrainAura;
    public float drainDuration = 2.5f;
    public int drainDamage = 1;
    public float drainTickRate = 0.4f;
    public float drainCooldown = 6f;

    private bool canLifeDrain = true;

    // attack for phase 2
    [Header("Bat Swarm Attack (Phase 2)")]
    public GameObject batSwarmPrefab;
    public float batSwarmCooldown = 7f;
    private bool canSummonBats = true;

    // phase 2 buffs
    [Header("Phase 2 Settings")]
    public bool phase2 = false;
    public float phase2MoveSpeed = 3.2f;
    public float phase2WindCooldownMultiplier = 0.7f;
    public float phase2WindSpeedMultiplier = 1.4f;

    public float phase2ShakeDuration = 1f;
    public float phase2ShakeAmount = 0.25f;

    private SpriteRenderer mainSprite;

    void Start()
    {
        startPos = transform.position;

        currentHealth = maxHealth;
        fullBarWidth = healthBarFill.localScale.x;
        fillHeight = healthBarFill.localScale.y;

        fillSprite = healthBarFill.GetComponent<SpriteRenderer>();
        mainSprite = GetComponent<SpriteRenderer>();
        flash = GetComponent<DamageFlash>();

        if (lifeDrainAura != null)
            lifeDrainAura.SetActive(false);
    }

    void Update()
    {
        MovePattern();
        UpdateHealthBar();

        if (isAttacking) return;

        if (!phase2)
        {
            TryWindSlash();
        }
        else
        {
            TryWindSlash();    // still happens in phase 2
            TryLifeDrain();
            TrySummonBats();
        }
    }

    // movement system
    void MovePattern()
    {
        if (isAttacking) return;

        moveTime += Time.deltaTime * moveSpeed;
        float offsetY = Mathf.Sin(moveTime) * hoverHeight;

        transform.position = new Vector3(
            transform.position.x,
            startPos.y + offsetY,
            transform.position.z
        );
    }

    // health system
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        flash?.Flash();

        if (!phase2 && currentHealth <= maxHealth * 0.5f)
            StartCoroutine(EnterPhase2());

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

    // wind attack with the wings
    void TryWindSlash()
    {
        if (!canWindSlash) return;

        float dist = Mathf.Abs(transform.position.x - player.position.x);

        if (dist < windMinDist)
            StartCoroutine(WindSlashRoutine());
    }

    IEnumerator WindSlashRoutine()
    {
        canWindSlash = false;

        isAttacking = true;

        // short wind-up shake
        Vector3 original = transform.position;
        float t = 0.2f;
        while (t > 0)
        {
            transform.position = original + new Vector3(Random.Range(-0.05f, 0.05f), 0, 0);
            t -= Time.deltaTime;
            yield return null;
        }
        transform.position = original;

        // shoot wind slash projectile
        float direction = player.position.x < transform.position.x ? -1f : 1f;

        GameObject slash = Instantiate(windSlashPrefab, windShootPoint.position, Quaternion.identity);
        slash.GetComponent<WindSlash>()?.SetVelocity(new Vector2(direction * windSlashSpeed, 0));

        isAttacking = false;

        yield return new WaitForSeconds(windSlashCooldown);
        canWindSlash = true;
    }

    //the life drain attack
    void TryLifeDrain()
    {
        if (canLifeDrain)
            StartCoroutine(LifeDrainRoutine());
    }

    IEnumerator LifeDrainRoutine()
    {
        canLifeDrain = false;
        isAttacking = true;

        // +aura
        lifeDrainAura.SetActive(true);
        float tickTimer = 0f;
        float elapsed = 0f;

        while (elapsed < drainDuration)
        {
            // damage only if player walks into big aura
            if (Vector2.Distance(player.position, transform.position) < 3.5f)
            {
                tickTimer += Time.deltaTime;
                if (tickTimer >= drainTickRate)
                {
                    player.GetComponentInParent<HealthSystem>()?.TakeDamage(drainDamage);
                    tickTimer = 0f;
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        lifeDrainAura.SetActive(false);

        isAttacking = false;

        yield return new WaitForSeconds(drainCooldown);
        canLifeDrain = true;
    }

    // phase 2 bat attack
    void TrySummonBats()
    {
        if (canSummonBats)
            StartCoroutine(BatSwarmRoutine());
    }

    IEnumerator BatSwarmRoutine()
    {
        canSummonBats = false;
        isAttacking = true;

        // little shake telegraph
        Vector3 origin = transform.position;
        float t = 0.25f;
        while (t > 0)
        {
            transform.position = origin + new Vector3(Random.Range(-0.07f, 0.07f), 0, 0);
            t -= Time.deltaTime;
            yield return null;
        }
        transform.position = origin;

        // summon swarm
        Instantiate(batSwarmPrefab, transform.position, Quaternion.identity);

        isAttacking = false;

        yield return new WaitForSeconds(batSwarmCooldown);
        canSummonBats = true;
    }

   // phase 2 transformation
    IEnumerator EnterPhase2()
    {
        phase2 = true;
        isAttacking = true;

        Vector3 origin = transform.position;
        float t = phase2ShakeDuration;

        while (t > 0)
        {
            transform.position = origin + new Vector3(
                Random.Range(-phase2ShakeAmount, phase2ShakeAmount),
                Random.Range(-phase2ShakeAmount, phase2ShakeAmount),
                0
            );
            t -= Time.deltaTime;
            yield return null;
        }

        transform.position = origin;

        // speed buffs
        moveSpeed = phase2MoveSpeed;
        windSlashCooldown *= phase2WindCooldownMultiplier;
        windSlashSpeed *= phase2WindSpeedMultiplier;

        // visual tint
        if (mainSprite != null)
            mainSprite.color = new Color(1f, 0.4f, 0.4f);

        isAttacking = false;
    }
}
