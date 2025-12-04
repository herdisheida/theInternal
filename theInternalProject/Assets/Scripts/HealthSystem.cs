using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Health Bar (World Space)")]
    public GameObject healthBarRoot;   // The whole health bar object (parent)
    public Transform healthBarFill;    // Only the green fill object
    public float smoothSpeed = 10f;    // Smooth scaling speed

    [Header("Health Text (Optional)")]
    public TextMeshProUGUI healthText;       // UI text (e.g. on screen)
    public TextMeshPro healthWorldText;      // World-space text above player

    [Header("Invincibility framees")]

    public float invincibilityPeriod = 0.9f;
    public float preBlinkDelay = 0.1f; // To show Damage Flash
    private bool isInvincible = false;
    public float blinkInterval = 0.1f;
    private SpriteRenderer[] spriteRenderers;


    void Start()
    {
        currentHealth = maxHealth;
        UpdateAllHealthDisplays();

        // Cache all sprites on load for better perfomance
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    // Called when the player takes damage
    public void TakeDamage(int amount)
    {
        if (isInvincible) return; // no damage if invincible

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth); // prevent negative health

        // Flash red when hit
        DamageFlash flash = GetComponent<DamageFlash>();
        if (flash != null)
            flash.Flash();

        UpdateAllHealthDisplays();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }
    
    // about 1 seconds of iframes
    System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        if (preBlinkDelay > 0f)
        {
            // so damage flash can be shown
            yield return new WaitForSeconds(preBlinkDelay);
        }
        float elapsed = 0f;
        bool visible = true;

        while (elapsed < invincibilityPeriod)
        {   
            //Toggle visibility on and off
            visible = !visible;

            // For all sprites connected to main character
            foreach(var sr in spriteRenderers)
            {
                if (sr != null)
                {
                    sr.enabled = visible;
                }
            }

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }
        foreach(var sr in spriteRenderers)
        {
            if (sr != null)
            {
                sr.enabled = true;
            }
        }

        // Can get hurt again
        isInvincible = false;

    }
    void Update()
    {
        UpdateHealthBar();
    }

    // Updates the world-space health bar
    void UpdateHealthBar()
    {
        if (healthBarRoot == null || healthBarFill == null)
            return;

        // Hide bar when full, show when damaged
        healthBarRoot.SetActive(currentHealth < maxHealth);

        float targetRatio = (float)currentHealth / maxHealth;

        // Smooth shrink from right to left (pivot must be on the right)
        float currentScaleX = healthBarFill.localScale.x;
        float newScaleX = Mathf.Lerp(currentScaleX, targetRatio, Time.deltaTime * smoothSpeed);

        healthBarFill.localScale = new Vector3(newScaleX, 1f, 1f);
    }

    // Updates health number (UI or world)
    void UpdateHealthText()
    {
        if (healthText != null)
            healthText.text = currentHealth.ToString();

        if (healthWorldText != null)
            healthWorldText.text = currentHealth.ToString();
    }

    void UpdateAllHealthDisplays()
    {
        UpdateHealthBar();
        UpdateHealthText();
    }

    public void Die()
    {
        // hide health bar on death
        if (healthBarRoot != null)
            healthBarRoot.SetActive(false);

        // deactivate the player
        gameObject.SetActive(false);
        SceneManager.LoadScene("ShootPatient");
    }
}
