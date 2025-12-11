using UnityEngine;
using System.Collections;

public class Vampire : MonoBehaviour
{
    public Transform player;

    // -------- MOVEMENT --------
    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float hoverAmplitude = 1.2f;
    public float hoverFrequency = 1.5f;
    private float moveTime = 0f;

    private bool busy = false;
    private Vector3 startPos;

    // -------- HEALTH --------
    [Header("Health")]
    public int maxHealth = 120;
    private int currentHealth;

    public Transform healthBarFill;
    private SpriteRenderer barSprite;
    private float fullWidth;
    private float barHeight;
    private DamageFlash flash;

    // -------- WIND SLASH --------
    [Header("Wind Slash")]
    public GameObject windSlashPrefab;
    public Transform shootPoint;
    public float windCooldown = 2f;
    public float slashSpeed = 7f;
    public float windAttackDist = 12f;
    private bool canWind = true;

    // -------- BAT SWARM --------
    [Header("Bat Swarm")]
    public GameObject batPrefab;
    public int batsToSpawn = 4;
    public float swarmCooldown = 7f;
    private bool canSwarm = true;

    // -------- GRAVITY PULL --------
    [Header("Gravity Pull")] 
    public float pullDuration = 3f;
    public float pullStrength = 7f;
    public float pullRange = 15f;
    public float gravityCooldown = 10f;

    private bool canGravityPull = true;

    private Vector2 externalForce = Vector2.zero;

    [Header("Gravity pull Wave")]
    public GameObject CircleWavePrefab;
    public int wavesPerPull = 3;
    public float timeBetweenWaves = 0.7f;





    // -------- PHASE 2 --------
    public bool phase2 = false;

    void Start()
    {
        startPos = transform.position;

        currentHealth = maxHealth;

        barSprite = healthBarFill.GetComponent<SpriteRenderer>();
        fullWidth = healthBarFill.localScale.x;
        barHeight = healthBarFill.localScale.y;

        flash = GetComponent<DamageFlash>();
    }

    void Update()
    {
        HoverMove();
        UpdateHealthBar();

        if (busy) return;

        // --- Phase 1 ---
        if (!phase2)
        {
            TryWindSlash();
        }
        else
        {
            // --- Phase 2 attacks ---
            TryWindSlash();
            TryBatSwarm();
            TryGravityPull();
        }

        // Apply gravity pull and knockback movement if present
        if (externalForce.magnitude > 0.1f)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.MovePosition(rb.position + externalForce * Time.deltaTime);
            }

            // Smoothly decay the force
            externalForce = Vector2.Lerp(externalForce, Vector2.zero, 4f * Time.deltaTime);
        }


    }

    // -------- MOVEMENT --------
    void HoverMove()
    {
        if (busy) return;

        moveTime += Time.deltaTime;

        // VERTICAL float motion
        float yOffset = Mathf.Sin(moveTime * hoverFrequency) * hoverAmplitude;

        // SMALL horizontal breathing movement (feels alive)
        float xOffset = Mathf.Cos(moveTime * 0.7f) * 0.3f;

        // Fixed right-side anchor position
        float rightSideX = 6.5f;   // adjust to your arena layout

        Vector3 targetPos = new Vector3(
            rightSideX + xOffset,
            startPos.y + yOffset,
            transform.position.z
        );

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 3f);
    }



    // -------- HEALTH --------
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        flash?.Flash();

        if (!phase2 && currentHealth <= maxHealth * 0.5f)
        {
            StartCoroutine(EnterPhase2());
        }

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    void UpdateHealthBar()
    {
        float ratio = (float)currentHealth / maxHealth;
        float newX = Mathf.Lerp(healthBarFill.localScale.x, fullWidth * ratio, Time.deltaTime * 10f);

        healthBarFill.localScale = new Vector3(newX, barHeight, 1f);
        barSprite.color = ratio < 0.25f ? Color.red : Color.green;
    }


    // ============================================================
    //                       WIND SLASH
    // ============================================================
    void TryWindSlash()
    {
        if (!canWind) return;

        float dist = Mathf.Abs(player.position.x - transform.position.x);
        if (dist < windAttackDist)
            StartCoroutine(WindSlashRoutine());
    }

    IEnumerator WindSlashRoutine()
    {
        busy = true;
        canWind = false;

        yield return new WaitForSeconds(0.2f);

        float dir = (player.position.x < transform.position.x) ? -1f : 1f;
        GameObject slash = Instantiate(windSlashPrefab, shootPoint.position, Quaternion.identity);
        slash.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(dir * slashSpeed, 0);

        busy = false;
        yield return new WaitForSeconds(windCooldown);
        canWind = true;
    }


    // ============================================================
    //                       BAT SWARM
    // ============================================================
    void TryBatSwarm()
    {
        if (canSwarm)
            StartCoroutine(BatSwarmRoutine());
    }

    IEnumerator BatSwarmRoutine()
    {
        busy = true;
        canSwarm = false;

        yield return new WaitForSeconds(0.2f);

        // Spawn multiple bats
        for (int i = 0; i < batsToSpawn; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            Instantiate(batPrefab, transform.position + offset, Quaternion.identity);
        }

        busy = false;
        yield return new WaitForSeconds(swarmCooldown);
        canSwarm = true;
    }


    // ======================================================================
    // GRAVITY PULL ATTACK (Phase 2)
    // ======================================================================

    void TryGravityPull()
    {
        if (canGravityPull)
            StartCoroutine(GravityPullRoutine());
    }

    IEnumerator GravityPullRoutine()
    {
        canGravityPull = false;
        busy = true;

        float timer = 0f;
        int wavesEmitted = 0;

        while (timer < pullDuration)
        {
            float dist = Vector2.Distance(player.position, transform.position);

            // gravity pull 
            {
                Vector2 dir = (transform.position - player.position).normalized;
                var hp = player.GetComponentInParent<HealthSystem>();
                hp?.ApplyExternalForce(dir * pullStrength * Time.deltaTime);
            }

            // Circle Waves
            if (wavesEmitted < wavesPerPull && timer >= wavesEmitted * timeBetweenWaves)
            {
                Instantiate(CircleWavePrefab, transform.position, Quaternion.identity);
                wavesEmitted++;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        busy = false;

        yield return new WaitForSeconds(gravityCooldown);
        canGravityPull = true;
    }




    // ============================================================
    //                      PHASE 2 TRANSITION
    // ============================================================
    IEnumerator EnterPhase2()
    {
        phase2 = true;
        busy = true;

        // small shake
        Vector3 orig = transform.position;
        float t = 0.5f;

        while (t > 0)
        {
            transform.position = orig + (Vector3)Random.insideUnitCircle * 0.15f;
            t -= Time.deltaTime;
            yield return null;
        }

        transform.position = orig;

        // Visual effect
        GetComponent<SpriteRenderer>().color = new Color(1f, 0.4f, 0.4f);

        busy = false;
    }
}
