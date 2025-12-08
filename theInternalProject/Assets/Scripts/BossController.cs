using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    public Transform player;

    [Header("Attack Start Delay")]
    public float attackDelay = 3f;   // wait this long before any attacks
    private float attackTimer = 0f;
    private bool attacksEnabled = false;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float moveDistance = 2f;
    private Vector3 startPos;
    private float movementTime = 0f;
    private bool freezeMovement = false;

    [Header("Health")]
    public int maxHealth = 50;
    private int currentHealth;
    public Transform healthBarFill;
    public GameObject healthBarRoot;

    [Header("Health Bar Smoothness")]
    public float smoothSpeed = 10f;

    // New variables to store the starting shape
    private float fullBarWidth;
    private float fillHeight;

    private SpriteRenderer fillSprite;

    [Header("Shooting Attack")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public int bulletsPerBurst = 4;
    public float timeBetweenShots = 0.15f;
    public float burstInterval = 2f;
    private bool isBursting = false;

    [Header("Bite Attack")]
    public GameObject biteHitbox;
    public float lungeDistance = 0.5f;
    public float lungeSpeed = 10f;
    public float biteCooldown = 2f;
    public float biteActiveTime = 0.15f;
    private bool canBite = true;
    private bool isBiting = false;

    [Header("Bite Telegraph")]
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.1f;

    [Header("Phase Control")]
    public bool phase2 = false;
    public float phase2Threshold = 0.5f; // health below 50 percent

    [Header("Phase 2 Vine Attack")]
    public GameObject vinePrefab;
    public float vineInterval = 2.5f;
    public float vineDamage = 10f;
    private bool isUsingVines = false;

    [Header("Spread Shot Attack")]
    public GameObject spreadBulletPrefab;
    public int spreadCount = 5;
    public float spreadAngle = 45f;
    public float spreadCooldown = 4f;
    private bool canSpread = true;

    void Start()
    {
        AudioManager.instance?.PlayZombieBossBattleMusic();

        
        startPos = transform.position;
        currentHealth = maxHealth;

        if (biteHitbox != null)
            biteHitbox.SetActive(false);

        // Prepare health bar scaling
        fullBarWidth = healthBarFill.localScale.x;
        fillHeight = healthBarFill.localScale.y;

        fillSprite = healthBarFill.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        MoveBoss();
        UpdateHealthBar();

        // ----- delay attacks for a few seconds -----
        if (!attacksEnabled)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer < attackDelay)
                return;                    // boss moves only + no attacks yet
            attacksEnabled = true;
        }
        // ------------------------------------------

        if (!phase2)
        {
            if (!isBursting)
                StartCoroutine(ShootBurst());

            float distance = Vector2.Distance(transform.position, player.position);
            if (distance < 7f && canBite)
                StartCoroutine(BiteAttackRoutine());
        }
        else
        {
            if (canSpread)
                StartCoroutine(SpreadShotRoutine());
        }
    }


    // ---------------- MOVEMENT ----------------
    void MoveBoss()
    {
        if (freezeMovement) return;

        movementTime += Time.deltaTime * moveSpeed;
        float offsetY = Mathf.Sin(movementTime) * moveDistance;

        transform.position = new Vector3(
            transform.position.x,
            startPos.y + offsetY,
            transform.position.z
        );
    }


    // ---------------- SHOOTING ----------------
    IEnumerator ShootBurst()
    {
        isBursting = true;

        for (int i = 0; i < bulletsPerBurst; i++)
        {
            Shoot();
            yield return new WaitForSeconds(timeBetweenShots);
        }

        yield return new WaitForSeconds(burstInterval);
        isBursting = false;
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    }


    // ---------------- HEALTH ----------------
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        GetComponent<DamageFlash>().Flash();

        float hpPercent = (float)currentHealth / maxHealth;

        // Shake camera under 25 percent HP
        if (hpPercent <= 0.25f)
        {
            CameraShake.instance?.Shake(0.3f, 0.15f);
        }

        if (!phase2 && currentHealth > 0 && currentHealth <= maxHealth * phase2Threshold)
        {
            EnterPhase2();
        }

        if (currentHealth <= 0)
        {
            healthBarRoot?.SetActive(false);
            Die();
            PatientAnalysisScreen.isSaved = true;
            SceneManager.LoadScene("ZombieAnalysisScreen");
            return;
        }
    }



    void UpdateHealthBar()
    {
        float ratio = (float)currentHealth / maxHealth;

        // Smooth width animation only
        float currentX = healthBarFill.localScale.x;
        float targetX = fullBarWidth * ratio;
        float smoothedX = Mathf.Lerp(currentX, targetX, Time.deltaTime * smoothSpeed);

        // Apply scale BUT keep your manual height
        healthBarFill.localScale = new Vector3(
            smoothedX,
            fillHeight,
            healthBarFill.localScale.z
        );

        // Optional: turn bar red at low HP
        if (fillSprite != null)
        {
            fillSprite.color = (ratio <= 0.25f ? Color.red : Color.green);
        }
    }


    void Die()
    {
        StopAllCoroutines();
        Destroy(gameObject);
        SceneManager.LoadScene("PatientSelection");
    }


    // ---------------- BITE ATTACK ----------------
    IEnumerator BiteAttackRoutine()
    {
        if (currentHealth <= 0) yield break;

        canBite = false;
        isBiting = true;
        freezeMovement = true;

        Vector3 originalPos = transform.position;

        // Telegraph shake
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            if (currentHealth <= 0) yield break;

            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-0.5f, 0.5f) * shakeMagnitude;

            transform.position = originalPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;

        float direction = (player.position.x < transform.position.x) ? -1f : 1f;
        Vector3 bitePos = originalPos + new Vector3(direction * lungeDistance, 0, 0);

        float t = 0;
        while (t < 1f)
        {
            if (currentHealth <= 0) yield break;

            t += Time.deltaTime * lungeSpeed;
            transform.position = Vector3.Lerp(originalPos, bitePos, t);
            yield return null;
        }

        AudioManager.instance?.ZombieChomp();
        biteHitbox.SetActive(true);
        yield return new WaitForSeconds(biteActiveTime);
        biteHitbox.SetActive(false);

        t = 0;
        while (t < 1f)
        {
            if (currentHealth <= 0) yield break;

            t += Time.deltaTime * lungeSpeed;
            transform.position = Vector3.Lerp(bitePos, originalPos, t);
            yield return null;
        }

        yield return new WaitForSeconds(biteCooldown);

        freezeMovement = false;
        isBiting = false;
        canBite = true;
    }


    // ---------------- PHASE 2 ----------------
    void EnterPhase2()
    {
        if (currentHealth <= 0) return;

        phase2 = true;

        isBursting = true;
        canBite = false;
        isBiting = false;

        StartCoroutine(VineAttackRoutine());
    }

    IEnumerator VineAttackRoutine()
    {
        isUsingVines = true;

        while (phase2)
        {
            if (currentHealth <= 0) yield break;

            float topY = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1.3f, 0)).y;

            Vector3 spawnPos = new Vector3(
                player.position.x,
                topY,
                0f
            );

            GameObject vine = Instantiate(vinePrefab, spawnPos, Quaternion.identity);
            vine.GetComponent<VineAttack>().damage = (int)vineDamage;

            yield return new WaitForSeconds(vineInterval);
        }

        isUsingVines = false;
    }


    IEnumerator SpreadShotRoutine()
    {
        canSpread = false;

        float angleStep = spreadAngle / (spreadCount - 1);
        float startAngle = 180f - (spreadAngle / 2f);

        for (int i = 0; i < spreadCount; i++)
        {
            float angle = startAngle + angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            GameObject bullet = Instantiate(spreadBulletPrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<SpreadBullet>().SetDirection(dir);
        }

        yield return new WaitForSeconds(spreadCooldown);
        canSpread = true;
    }
}

