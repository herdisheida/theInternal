using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Vampire : MonoBehaviour
{
    public Transform player;
    private bool isDead = false;


    // movement settings
    [Header("Movement")]
    public float hoverAmplitude = 1.2f;
    public float hoverFrequency = 1.5f;
    private float moveTime = 0f;
    private Vector3 startPos;

    private bool busy = false;   // is the boss busy performing an action?

    //heatl settings
    [Header("Health")]
    public int maxHealth = 120;
    private int currentHealth;

    public Transform healthBarFill;
    private SpriteRenderer barSprite;
    private float fullWidth;
    private float barHeight;
    private DamageFlash flash;

    // wind slash settings
    [Header("Wind Slash")]
    public GameObject windSlashPrefab;
    public Transform shootPoint;
    public float windCooldown = 2.5f;
    public float slashSpeed = 7f;

    // bat swarm settings
    [Header("Bat Swarm")]
    public GameObject batPrefab;
    public int batsToSpawn = 5;
    public float swarmCooldown = 5f;

    // gravity pull settings
    [Header("Gravity Pull")] 
    public float pullDuration = 2.5f;
    public float pullStrength = 7f;
    public float gravityCooldown = 8f;
    public GameObject circleWavePrefab;
    public int wavesPerPull = 3;
    public float timeBetweenWaves = 0.6f;

    [Header("Death Animation")]
    public float deathShakeDuration = 1f;
    public float deathShakeMagnitude = 0.12f;
    public float deathFallSpeed = 6f;
    public float deathFallRotationSpeed = 180f;   // degrees per second
    public float deathFallDistance = 6f;          // how far down he falls
    public string deathNextScene = "AnalysisScreen";

    private Vector2 externalForce = Vector2.zero;

    // phase 2
    public bool phase2 = false;

    void Start()
    {
        startPos = transform.position;
        currentHealth = maxHealth;

        barSprite = healthBarFill.GetComponent<SpriteRenderer>();
        fullWidth = healthBarFill.localScale.x;
        barHeight = healthBarFill.localScale.y;
        flash = GetComponent<DamageFlash>();

        StartCoroutine(PhaseOneAttackLoop());
    }

    void Update()
    {
        if (isDead) return; // make sure it doesnt move when dead

        HoverMotion();
        UpdateHealthBar();

        if (externalForce.magnitude > 0.1f)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.MovePosition(rb.position + externalForce * Time.deltaTime);

            externalForce = Vector2.Lerp(externalForce, Vector2.zero, 4f * Time.deltaTime);
        }
    }


    // movement
    void HoverMotion()
    {
        if (isDead) return;

        moveTime += Time.deltaTime;

        float yOffset = Mathf.Sin(moveTime * hoverFrequency) * hoverAmplitude;
        float xOffset = Mathf.Cos(moveTime * 0.6f) * 0.4f;

        float rightX = 6.5f;
        transform.position = new Vector3(rightX + xOffset, startPos.y + yOffset, transform.position.z);
    }



    // health
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        flash?.Flash();

        // Check for phase change
        if (!phase2 && currentHealth <= maxHealth * 0.5f)
        {
            phase2 = true;
            AudioManager.instance?.VampireGrowl();
            StopAllCoroutines();
            StartCoroutine(PhaseTwoAttackLoop());
        }

        if (currentHealth <= 0)
            TriggerDeath();

    }

    void UpdateHealthBar()
    {
        // Smoothly update health bar scale and color
        float ratio = (float)currentHealth / maxHealth;
        float newX = Mathf.Lerp(healthBarFill.localScale.x, fullWidth * ratio, Time.deltaTime * 10f);

        healthBarFill.localScale = new Vector3(newX, barHeight, 1f);
        barSprite.color = ratio < 0.25f ? Color.red : Color.green;
    }


    // phase 1 attack loop (only wind slash)
    IEnumerator PhaseOneAttackLoop()
    {
        while (!phase2)
        {
            yield return WindSlashRoutine();
            yield return new WaitForSeconds(windCooldown);
        }
    }


    //phase 2 attack loop (all attacks)
    IEnumerator PhaseTwoAttackLoop()
    {
        while (true)
        {
            yield return WindSlashRoutine();
            yield return new WaitForSeconds(0.5f);

            yield return BatSwarmRoutine();
            yield return new WaitForSeconds(0.5f);

            yield return GravityPullRoutine();
            yield return new WaitForSeconds(1f);
        }
    }


    // wind slash
    IEnumerator WindSlashRoutine()
    {
        if (busy) yield break;
        busy = true;

        yield return new WaitForSeconds(0.2f);

        // shoot slash toward player
        float dir = (player.position.x < transform.position.x) ? -1f : 1f;
        GameObject slash = Instantiate(windSlashPrefab, shootPoint.position, Quaternion.identity);
        slash.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(dir * slashSpeed, 0);

        busy = false;

        // shake and particles
        CameraShake.instance?.Shake(0.1f, 0.05f);

        float telegraphTime = 0.25f;
        float t = 0f;
        Vector3 original = transform.position;

        while (t < telegraphTime)
        {
            transform.position = original + (Vector3)Random.insideUnitCircle * 0.05f;
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = original;

    }


    // bat swarm
    IEnumerator BatSwarmRoutine()
    {
        if (busy) yield break;
        busy = true;

        yield return new WaitForSeconds(0.3f);

        // spawn bats around vampire
        for (int i = 0; i < batsToSpawn; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-1.2f, 1.2f), Random.Range(-1f, 1f), 0);
            Instantiate(batPrefab, transform.position + offset, Quaternion.identity);
        }

        busy = false;
    }


    // gravity pull
    IEnumerator GravityPullRoutine()
    {
        if (busy) yield break;
        busy = true;

        float timer = 0f;
        int waves = 0;

        // pull player in over duration
        while (timer < pullDuration)
        {
            Vector2 dir = (transform.position - player.position).normalized;

            // apply pull force to player
            var hp = player.GetComponentInParent<HealthSystem>();
            hp?.ApplyExternalForce(dir * pullStrength * Time.deltaTime);

            // spawn shockwave circles at intervals
            if (waves < wavesPerPull && timer >= waves * timeBetweenWaves)
            {
                Instantiate(circleWavePrefab, transform.position, Quaternion.identity);
                waves++;
                CameraShake.instance?.Shake(0.2f, 0.12f);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        busy = false;
    }

    public void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;

        // stop everything BEFORE starting the death sequence
        StopAllCoroutines();

        AudioManager.instance?.FadeOutMusic(1f);
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        busy = true;

        AudioManager.instance?.VampireScream();

        // disable hitbox & physics
        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb) rb.simulated = false;

        AudioManager.instance?.ZombieDeath();
        CameraShake.instance?.Shake(0.4f, 0.2f);

        Vector3 originalPos = transform.position;

        // SHAKE
        float elapsed = 0f;
        while (elapsed < deathShakeDuration)
        {
            float x = Random.Range(-1f, 1f) * deathShakeMagnitude;
            float y = Random.Range(-1f, 1f) * deathShakeMagnitude;

            transform.position = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;

        // FALL
        Vector3 targetPos = originalPos + Vector3.down * deathFallDistance;
        while (transform.position.y > targetPos.y)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                deathFallSpeed * Time.deltaTime
            );

            transform.Rotate(0, 0, deathFallRotationSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(deathNextScene);
    }

}
