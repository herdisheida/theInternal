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
    public Transform healthBarFill;   // drag green fill bar here

    [Header("Shooting")]
    public Transform firePoint;       // drag your firepoint here
    public GameObject bulletPrefab;   // drag shot_attack prefab here

    public int bulletsPerBurst = 3;          // bullets per burst
    public float timeBetweenShots = 0.3f;   // delay between bullets
    public float burstInterval = 2f;         // delay between bursts
    private bool isBursting = false;

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

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        float ratio = (float)currentHealth / maxHealth;
        healthBarFill.localScale = new Vector3(ratio, 1f, 1f);
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
