using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;


public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    // Shared between scenes
    public static int sharedHealth = -1;

    [Header("Health Bar (World Space)")]
    public GameObject healthBarRoot;
    public Transform healthBarFill;
    public float smoothSpeed = 10f;
    private SpriteRenderer healthBarSprite;

    [Header("Low HP Pulse")]
    public float pulseSpeed = 6f;        
    public float pulseScaleAmount = 0.15f; 

    [Header("Health Text (Optional)")]
    public TextMeshProUGUI healthText;
    public TextMeshPro healthWorldText;

    [Header("Invincibility Frames")]
    public float invincibilityPeriod = 0.9f;
    public float preBlinkDelay = 0.1f;
    private bool isInvincible = false;
    public float blinkInterval = 0.07f;
    private SpriteRenderer[] spriteRenderers;
    private Vector2 externalForce = Vector2.zero;

    void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Start()
    {
        if (sharedHealth < 0 || sharedHealth > maxHealth)
        {
            currentHealth = maxHealth;
            sharedHealth = currentHealth;
        }
        else
        {
            currentHealth = sharedHealth;
        }

        UpdateAllHealthDisplays();

        healthBarSprite = healthBarFill.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        UpdateHealthBar();
    }


    // damage
    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        sharedHealth = currentHealth;

        // Flash red
        DamageFlash flash = GetComponent<DamageFlash>();
        if (flash != null)
            flash.Flash();

        UpdateAllHealthDisplays();

        if (currentHealth <= 0)
        {
            Die();
            AudioManager.instance?.FadeOutMusic(1.5f);
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }


    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        if (preBlinkDelay > 0)
            yield return new WaitForSeconds(preBlinkDelay);

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < invincibilityPeriod)
        {
            visible = !visible;

            foreach (var sr in spriteRenderers)
                if (sr != null) sr.enabled = visible;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        foreach (var sr in spriteRenderers)
            if (sr != null) sr.enabled = true;

        isInvincible = false;
    }

    public void ApplyExternalForce(Vector2 force)
    {
        externalForce += force;  // stackable pulls or pushes
    }


    //death 
    public void Die()
    {
        if (healthBarRoot != null)
            healthBarRoot.SetActive(false);

        gameObject.SetActive(false);
        GameManager.instance?.MarkPatientInfected();
        SceneManager.LoadScene("ShootPatient");
    }


    // health bar
    void UpdateHealthBar()
    {
        if (healthBarRoot == null || healthBarFill == null)
            return;

        // show only when damaged
        healthBarRoot.SetActive(currentHealth < maxHealth);

        float ratio = (float)currentHealth / maxHealth;

        // low HP pulse effect
        if (ratio <= 0.25f)
        {
            // turn red
            if (healthBarSprite != null)
                healthBarSprite.color = Color.red;

            // pulse height
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseScaleAmount;

            healthBarFill.localScale = new Vector3(
                healthBarFill.localScale.x,
                pulse,
                1f
            );

            // trigger screen-edge glow pulse
            ScreenVignettePulse.instance?.StartPulse();
        }
        else
        {
            // normal color
            if (healthBarSprite != null)
                healthBarSprite.color = Color.green;

            // reset bar height
            healthBarFill.localScale = new Vector3(
                healthBarFill.localScale.x,
                1f,
                1f
            );

            // stop glow pulse
            ScreenVignettePulse.instance?.StopPulse();
        }

        float currentX = healthBarFill.localScale.x;
        float targetX = ratio; 

        float newX = Mathf.Lerp(currentX, targetX, Time.deltaTime * smoothSpeed);

        // Keep only width changing
        healthBarFill.localScale = new Vector3(
            newX,
            healthBarFill.localScale.y, // keep pulse height if low HP
            1f
        );
    }



    // health text
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

    // should be knockback force
    public void ApplyKnockback(Vector2 force)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) return;

        // Apply knockback when windslash hits
        rb.linearVelocity = force;
    }

    public Vector2 ConsumeExternalForce()
    {
        Vector2 f = externalForce;
        externalForce = Vector2.Lerp(externalForce, Vector2.zero, 5f * Time.deltaTime);
        return f;
    }




}
