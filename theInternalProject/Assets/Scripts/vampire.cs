using UnityEngine;
using System.Collections;

public class Vampire : MonoBehaviour
{
    public Transform player;

    // -------- MOVEMENT --------
    [Header("Movement")]
    public float moveSpeed = 1.6f;
    public float hoverAmplitude = 1.2f;
    public float hoverFrequency = 1.4f;

    private float hoverTime = 0f;
    private Vector3 startPos;

    private bool busy = false;  // prevents actions during attacks
    private bool isDead = false;

    // -------- HEALTH --------
    [Header("Health")]
    public int maxHealth = 120;
    private int currentHealth;

    public Transform healthBarFill;
    private float fullWidth;
    private float height;
    private SpriteRenderer barSprite;
    private DamageFlash flash;

    // -------- WIND SLASH --------
    [Header("Wind Slash Attack")]
    public GameObject windSlashPrefab;
    public Transform shootPoint;
    public float windCooldown = 2f;
    public float slashSpeed = 8f;
    public float attackDistance = 12f;
    private bool canWind = true;

    // -------- LIFE DRAIN --------
    [Header("Life Drain Aura")]
    public GameObject drainAura;
    public float drainDuration = 2.5f;
    public float followSpeed = 10f;
    public float returnSpeed = 3f;
    public float drainRange = 3.2f;
    public int drainDamage = 1;
    public float tickRate = 0.4f;
    public float drainCooldown = 6f;
    private bool canDrain = true;
    public float auraDrainRadius = 3.2f;

    private Vector3 auraHomePos;

    // -------- BAT SWARM --------
    [Header("Bat Swarm Attack")]
    public GameObject swarmPrefab;
    public float swarmCooldown = 6f;
    private bool canSwarm = true;

    // -------- PHASE 2 --------
    [Header("Phase 2")]
    public bool phase2 = false;
    public float phase2MoveSpeed = 2.4f;
    public float phase2SlashSpeedMult = 1.3f;
    public float phase2WindCooldownMult = 0.7f;

    void Start()
    {
        startPos = transform.position;
        currentHealth = maxHealth;

        fullWidth = healthBarFill.localScale.x;
        height = healthBarFill.localScale.y;

        barSprite = healthBarFill.GetComponent<SpriteRenderer>();
        flash = GetComponent<DamageFlash>();

        if (drainAura != null)
        {
            auraHomePos = drainAura.transform.localPosition;
            drainAura.SetActive(false);
        }
    }

    void Update()
    {
        if (!busy && !isDead)
        {
            HoverMovement();
        }

        UpdateHealthBar();

        if (busy || isDead) return;

        if (!phase2)
        {
            TryWind();
        }
        else
        {
            TryWind();
            TryLifeDrain();
            TrySwarm();
        }
    }

    // -------------------------------------------
    // MOVEMENT
    // -------------------------------------------
    void HoverMovement()
    {
        hoverTime += Time.deltaTime;

        float offsetY = Mathf.Sin(hoverTime * hoverFrequency) * hoverAmplitude;
        float offsetX = Mathf.Cos(hoverTime * 0.5f) * 0.5f;

        Vector3 target = new Vector3(
            startPos.x + offsetX,
            startPos.y + offsetY,
            transform.position.z
        );

        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 2f);
    }

    // -------------------------------------------
    // HEALTH SYSTEM
    // -------------------------------------------
    public void TakeDamage(int amount)
    {
        if (isDead) return;

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
        float p = (float)currentHealth / maxHealth;

        float newX = Mathf.Lerp(healthBarFill.localScale.x, fullWidth * p, Time.deltaTime * 9f);
        healthBarFill.localScale = new Vector3(newX, height, 1f);

        barSprite.color = p <= 0.25f ? Color.red : Color.green;
    }

    void Die()
    {
        isDead = true;
        Destroy(gameObject, 0.1f);
    }

    // -------------------------------------------
    // WIND SLASH
    // -------------------------------------------
    void TryWind()
    {
        if (!canWind) return;

        float dist = Mathf.Abs(player.position.x - transform.position.x);

        if (dist < attackDistance)
            StartCoroutine(WindSlashRoutine());
    }

    IEnumerator WindSlashRoutine()
    {
        busy = true;
        canWind = false;

        yield return new WaitForSeconds(0.15f);

        float dir = (player.position.x < transform.position.x ? -1f : 1f);

        GameObject slash = Instantiate(windSlashPrefab, shootPoint.position, Quaternion.identity);
        slash.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(dir * slashSpeed, 0);

        busy = false;
        yield return new WaitForSeconds(windCooldown);
        canWind = true;
    }

    // -------------------------------------------
    // LIFE DRAIN AURA — FOLLOWS PLAYER
    // -------------------------------------------
    void TryLifeDrain()
    {
        if (canDrain)
            StartCoroutine(DrainRoutine());
    }
   IEnumerator DrainRoutine()
    {
        canDrain = false;
        busy = true;

        // Enable aura
        drainAura.SetActive(true);

        // Vampire moves faster during drain
        float drainSpeed = 3.5f;

        float elapsed = 0f;

        while (elapsed < drainDuration)
        {
            // Always follow the player
            Vector3 targetPos = Vector3.MoveTowards(
                transform.position,
                player.position,
                drainSpeed * Time.deltaTime
            );

            transform.position = targetPos;

            // Damage if player is inside aura collider
            float dist = Vector2.Distance(transform.position, player.position);

            if (dist < auraDrainRadius)  // define radius at top of script
            {
                player.GetComponentInParent<HealthSystem>()?.TakeDamage(drainDamage);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Disable after attack
        drainAura.SetActive(false);

        busy = false;

        // Cooldown
        yield return new WaitForSeconds(drainCooldown);
        canDrain = true;
    }



    // -------------------------------------------
    // BAT SWARM
    // -------------------------------------------
    void TrySwarm()
    {
        if (canSwarm)
            StartCoroutine(SwarmRoutine());
    }

    IEnumerator SwarmRoutine()
    {
        canSwarm = false;
        busy = true;

        yield return new WaitForSeconds(0.2f);

        // Spawn multiple swarms
        int swarmCount = phase2 ? 5 : 2;
        for (int i = 0; i < swarmCount; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-2f, 2f), Random.Range(-1f, 1f), 0);
            Instantiate(swarmPrefab, transform.position + offset, Quaternion.identity);
        }

        busy = false;

        yield return new WaitForSeconds(swarmCooldown);
        canSwarm = true;
    }

    // -------------------------------------------
    // ENTER PHASE 2
    // -------------------------------------------
    IEnumerator EnterPhase2()
    {
        phase2 = true;
        busy = true;

        Vector3 origin = transform.position;
        float t = 0.8f;

        while (t > 0)
        {
            transform.position = origin + new Vector3(Random.Range(-0.1f, 0.1f), 0);
            t -= Time.deltaTime;
            yield return null;
        }

        transform.position = origin;

        // Buffs
        moveSpeed = phase2MoveSpeed;
        slashSpeed *= phase2SlashSpeedMult;
        windCooldown *= phase2WindCooldownMult;

        barSprite.color = Color.magenta;

        busy = false;
    }
}
