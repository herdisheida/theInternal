using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float moveDistance = 3f;

    private Vector3 startPos;
    private float movementTime = 0f;

    [Header("Health")]
    public int maxHealth = 10;
    private int currentHealth;

    public Transform healthBarFill;   // the green bar (Fill object)
    public GameObject healthBarRoot;  // the entire HealthBar object in hierarchy

    [Header("Shooting")]
    public Transform firePoint;       
    public GameObject bulletPrefab;   

    public int bulletsPerBurst = 3;
    public float timeBetweenShots = 0.3f;
    public float burstInterval = 2f;
    private bool isBursting = false;

    [Header("Health Bar Smoothness")]
    public float smoothSpeed = 10f;

    void Start()
    {
        startPos = transform.position;
        currentHealth = maxHealth;
    }

    void Update()
    {
        MoveBoss();
        UpdateHealthBar();

        if (!isBursting)
        {
            StartCoroutine(ShootBurst());
        }
    }

    // Movement
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

    // Burst Shooting
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

    // Health
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        // flash red when hit
        GetComponent<DamageFlash>().Flash();

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            if (healthBarRoot != null)
                healthBarRoot.SetActive(false);

            Die();
        }
    }

    void UpdateHealthBar()
    {
        float targetRatio = (float)currentHealth / maxHealth;

        float currentX = healthBarFill.localScale.x;
        float newX = Mathf.Lerp(currentX, targetRatio, Time.deltaTime * smoothSpeed);

        healthBarFill.localScale = new Vector3(newX, 1f, 1f);
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
