using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Health Bar")]
    public GameObject healthBarRoot;   // the entire health bar object
    public Transform healthBarFill;    // the green bar (with pivot on right)
    public float smoothSpeed = 10f;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar(); // ensure full bar at start
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        // Flash red on hit
        GetComponent<DamageFlash>().Flash();

        // Do not allow negative health
        currentHealth = Mathf.Max(0, currentHealth);

        // Update the bar immediately on damage
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarRoot != null && healthBarFill != null)
        {
            // Show the bar only when hurt
            healthBarRoot.SetActive(currentHealth < maxHealth);

            float targetScaleX = (float)currentHealth / maxHealth;

            Vector3 currentScale = healthBarFill.localScale;

            currentScale.x = Mathf.Lerp(currentScale.x, targetScaleX, Time.deltaTime * smoothSpeed);

            healthBarFill.localScale = currentScale;
        }
    }

    void Update()
    {
        UpdateHealthBar();
    }

    public void Die()
    {
        // Hide the health bar
        if (healthBarRoot != null)
            healthBarRoot.SetActive(false);

        // Disable the player (or boss)
        gameObject.SetActive(false);
    }
}
