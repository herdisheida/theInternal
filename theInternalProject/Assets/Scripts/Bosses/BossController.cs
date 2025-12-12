using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    public Transform player;
    private PatientData patientData;

    [Header("Attack Start Delay")]
    public float attackDelay = 3f;
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

    // ---------------- BITE ATTACK ----------------
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

    // -------------- PHASE CONTROL ----------------
    [Header("Phase Control")]
    public bool phase2 = false;
    public float phase2Threshold = 0.5f;
    public float phase2ShakeDuration = 1f;
    public float phase2ShakeAmount = 0.25f;

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

    // -------------- DEATH ----------------
    [Header("Death Animation")]
    public GameObject zombieVineHang;
    public float deathShakeDuration = 1f;
    public float deathShakeMagnitude = 0.12f;
    public float deathFallSpeed = 6f;
    public float deathFallRotationSpeed = 180f;   // degrees per second
    public float deathFallDistance = 6f;          // how far down he falls
    public string deathNextScene = "AnalysisScreen";

    private bool isDying = false;
    private Coroutine vineRoutine;
    private Coroutine shootBurstRoutine;



    void Start()
    {
        AudioManager.instance?.PlayZombieBossBattleMusic();

        startPos = transform.position;
        currentHealth = maxHealth;

        if (biteHitbox != null)
            biteHitbox.SetActive(false);

        fullBarWidth = healthBarFill.localScale.x;
        fillHeight = healthBarFill.localScale.y;
        fillSprite = healthBarFill.GetComponent<SpriteRenderer>();

        if (PatientManager.Instance != null) {
            patientData = PatientManager.Instance.selectedPatient;
        }
    }

    void Update()
    {
        MoveBoss();
        UpdateHealthBar();

        // delay before attacks
        if (!attacksEnabled)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer < attackDelay)
                return;
            attacksEnabled = true;
        }

        // ---- PHASE 1 ----
        if (!phase2)
        {
            if (!isBursting)
                StartCoroutine(ShootBurst());

            float distance = Vector2.Distance(transform.position, player.position);

            if (distance < 7f && canBite)
                StartCoroutine(BiteAttackRoutine());
        }

        // ---- PHASE 2 ----
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

        transform.position =
            new Vector3(transform.position.x, startPos.y + offsetY, transform.position.z);
    }

    // ---------------- SHOOTING ----------------
    IEnumerator ShootBurst()
    {
        isBursting = true;

        for (int i = 0; i < bulletsPerBurst; i++)
        {
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            yield return new WaitForSeconds(timeBetweenShots);
        }

        yield return new WaitForSeconds(burstInterval);
        isBursting = false;
    }

    // ---------------- BITE ATTACK ROUTINE ----------------
    IEnumerator BiteAttackRoutine()
    {
        if (currentHealth <= 0) yield break;

        canBite = false;
        isBiting = true;
        freezeMovement = true;

        Vector3 originalPos = transform.position;

        // TELEGRAPH SHAKE
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-0.5f, 0.5f) * shakeMagnitude;

            transform.position = originalPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;

        // LUNGE
        float direction = (player.position.x < transform.position.x) ? -1f : 1f;
        Vector3 bitePos = originalPos + new Vector3(direction * lungeDistance, 0, 0);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * lungeSpeed;
            transform.position = Vector3.Lerp(originalPos, bitePos, t);
            yield return null;
        }

        // BITE HITBOX
        biteHitbox.SetActive(true);
        AudioManager.instance?.ZombieChomp();
        yield return new WaitForSeconds(biteActiveTime);
        biteHitbox.SetActive(false);

        // RETURN TO POSITION
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * lungeSpeed;
            transform.position = Vector3.Lerp(bitePos, originalPos, t);
            yield return null;
        }

        freezeMovement = false;
        isBiting = false;

        // COOLDOWN
        yield return new WaitForSeconds(biteCooldown);
        canBite = true;
    }


    // ---------------- HEALTH ----------------
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        GetComponent<DamageFlash>().Flash();

        float hpPercent = (float)currentHealth / maxHealth;
        if (hpPercent <= 0.25f)
            CameraShake.instance?.Shake(0.3f, 0.15f);

        if (!phase2 && currentHealth > 0 && currentHealth <= maxHealth * phase2Threshold)
            StartCoroutine(EnterPhase2());

        if (currentHealth <= 0 && !isDying)
        {
            isDying = true;


            healthBarRoot?.SetActive(false);
            // if (patientData != null)
            // {
            //     patientData.isSaved = true;
            //     patientData.status = PatientStatus.Saved;
            // }
            // else
            // {
            //     Debug.LogWarning("BossController: patientData is null");
            // }

            GameManager.instance?.MarkPatientSaved();
            GameManager.instance.PatientsSavedDict["Zombie"] = true;
            StartCoroutine(Die());
        }
    }

    void UpdateHealthBar()
    {
        float ratio = (float)currentHealth / maxHealth;
        float currentX = healthBarFill.localScale.x;
        float targetX = fullBarWidth * ratio;

        float smoothedX =
            Mathf.Lerp(currentX, targetX, Time.deltaTime * smoothSpeed);

        healthBarFill.localScale =
            new Vector3(smoothedX, fillHeight, healthBarFill.localScale.z);

        fillSprite.color = (ratio <= 0.25f ? Color.red : Color.green);
    }

    IEnumerator Die()
    {
        // stop all ongoing attacks/movement
        attacksEnabled = false;
        freezeMovement = true;
        canBite = false;
        canSpread = false;
        isBursting = false;
        isUsingVines = false;
        if (vineRoutine != null) // stop vine attack
        {
            StopCoroutine(vineRoutine);
            vineRoutine = null;
        }
        if (shootBurstRoutine != null) // stop shooting
        {
            StopCoroutine(shootBurstRoutine);
            shootBurstRoutine = null;
        }

        // sound effect
        AudioManager.instance?.ZombieDeath();

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
        Destroy(zombieVineHang);
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
        yield return new WaitForSeconds(1f);
        StopAllCoroutines();
        SceneManager.LoadScene(deathNextScene);
    }

    // ---------------- PHASE 2 ----------------
    IEnumerator EnterPhase2()
    {
        AudioManager.instance?.ZombieRoar();
        phase2 = true;

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

        StartCoroutine(VineAttackRoutine());
    }

    IEnumerator VineAttackRoutine()
    {
        while (phase2 && !isDying)
        {
            float topY = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1.3f, 0)).y;

            Vector3 spawnPos = new Vector3(player.position.x, topY, 0f);
            GameObject vine = Instantiate(vinePrefab, spawnPos, Quaternion.identity);
            vine.GetComponent<VineAttack>().damage = (int)vineDamage;

            yield return new WaitForSeconds(vineInterval);
        }
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
