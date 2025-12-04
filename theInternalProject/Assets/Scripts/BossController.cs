using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class BossController : MonoBehaviour
{
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float moveDistance = 3f;
    private Vector3 startPos;
    private float movementTime = 0f;
    private bool freezeMovement = false;

    [Header("Health")]
    public int maxHealth = 10;
    private int currentHealth;
    public Transform healthBarFill;
    public GameObject healthBarRoot;

    [Header("Shooting Attack")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public int bulletsPerBurst = 3;
    public float timeBetweenShots = 0.3f;
    public float burstInterval = 2f;
    private bool isBursting = false;

    [Header("Health Bar Smoothness")]
    public float smoothSpeed = 10f;

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
    public float phase2Threshold = 0.5f; // health below 50%

    [Header("Phase 2 Vine Attack")]
    public GameObject vinePrefab;
    public float vineInterval = 2.5f;
    public float vineDamage = 10f;
    private bool isUsingVines = false;

    [Header("Spread Shot Attack")]
    public GameObject spreadBulletPrefab;
    public int spreadCount = 5; // how many bullets in the fan
    public float spreadAngle = 45f; // total degrees of the fan
    public float spreadCooldown = 4f;
    private bool canSpread = true;



    void Start()
    {
        startPos = transform.position;
        currentHealth = maxHealth;

        if (biteHitbox != null)
            biteHitbox.SetActive(false);
    }

    void Update()
    {
        MoveBoss();
        UpdateHealthBar();

        if (!phase2)
        {
            // PHASE 1 attacks
            if (!isBursting)
             StartCoroutine(ShootBurst());


            float distance = Vector2.Distance(transform.position, player.position);
            if (distance < 7f && canBite)
                StartCoroutine(BiteAttackRoutine());
        }
        else
        {
            // PHASE 2 attacks
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

        // Only enter phase 2 if boss is STILL alive
        if (!phase2 && currentHealth > 0 && currentHealth <= maxHealth * phase2Threshold)
        {
            EnterPhase2();
        }

        if (currentHealth <= 0)
        {
            if (healthBarRoot != null)
                healthBarRoot.SetActive(false);

            Die();
            return;
        }
    }

    void UpdateHealthBar()
    {
        float targetRatio = (float)currentHealth / maxHealth;
        float smoothedX = Mathf.Lerp(healthBarFill.localScale.x, targetRatio, Time.deltaTime * smoothSpeed);
        healthBarFill.localScale = new Vector3(smoothedX, 1f, 1f);
    }

    void Die()
    {
        StopAllCoroutines();
        Destroy(gameObject);

        // load next scene TODO HERDIS delay a little bit
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

            transform.position = new Vector3(
                originalPos.x + offsetX,
                originalPos.y + offsetY,
                originalPos.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;

        // Lunge toward player
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

        // Bite damage window
        AudioManager.instance?.ZombieChomp();
        biteHitbox.SetActive(true);
        yield return new WaitForSeconds(biteActiveTime);
        biteHitbox.SetActive(false);

        // Return
        t = 0;
        while (t < 1f)
        {
            if (currentHealth <= 0) yield break;

            t += Time.deltaTime * lungeSpeed;
            transform.position = Vector3.Lerp(bitePos, originalPos, t);
            yield return null;
        }

        // Cooldown
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
        Debug.Log("BOSS ENTERS PHASE 2!");

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

            // always spawn off-screen above the camera
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