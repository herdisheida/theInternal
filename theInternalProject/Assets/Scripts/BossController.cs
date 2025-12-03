using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float moveDistance = 3f;

    private Vector3 startPos;
    private float movementTime = 0f;

    [Header("Health")]
    public int maxHealth = 10;
    private int currentHealth;

    public Transform healthBarFill;
    public GameObject healthBarRoot;

    [Header("Shooting")]
    public Transform firePoint;
    public GameObject bulletPrefab;

    public int bulletsPerBurst = 3;
    public float timeBetweenShots = 0.3f;
    public float burstInterval = 2f;
    private bool isBursting = false;

    [Header("Health Bar Smoothness")]
    public float smoothSpeed = 10f;

    [Header("Bite Attack Settings")]
    public GameObject biteHitbox;
    public float lungeDistance = 0.5f;
    public float lungeSpeed = 10f;
    public float biteCooldown = 2f;
    public float biteActiveTime = 0.15f;
    private bool canBite = true;
    private bool isBiting = false;

    void Start()
    {
        startPos = transform.position;
        currentHealth = maxHealth;

        biteHitbox.SetActive(false); // important
    }

    void Update()
    {
        MoveBoss();
        UpdateHealthBar();

        if (!isBursting && !isBiting)
        {
            StartCoroutine(ShootBurst());
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < 2f && canBite)
        {
            StartCoroutine(BiteAttackRoutine());
        }
    }

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

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        GetComponent<DamageFlash>().Flash();

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            if (healthBarRoot != null)
                healthBarRoot.SetActive(false);

            Die();
        }
    }

    IEnumerator BiteAttackRoutine()
    {
        canBite = false;
        isBiting = true;

        Debug.Log("Boss is biting!");

        Vector3 originalPos = transform.position;

        // Determine direction
        float direction = player.position.x < transform.position.x ? -1f : 1f;

        Vector3 bitePos = originalPos + new Vector3(direction * lungeDistance, 0, 0);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * lungeSpeed;
            transform.position = Vector3.Lerp(originalPos, bitePos, t);
            yield return null;
        }

        biteHitbox.SetActive(true);
        yield return new WaitForSeconds(biteActiveTime);
        biteHitbox.SetActive(false);

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * lungeSpeed;
            transform.position = Vector3.Lerp(bitePos, originalPos, t);
            yield return null;
        }

        yield return new WaitForSeconds(biteCooldown);
        isBiting = false;
        canBite = true;
    }

    void UpdateHealthBar()
    {
        float targetRatio = (float)currentHealth / maxHealth;

        float newX = Mathf.Lerp(healthBarFill.localScale.x, targetRatio, Time.deltaTime * smoothSpeed);
        healthBarFill.localScale = new Vector3(newX, 1f, 1f);
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
