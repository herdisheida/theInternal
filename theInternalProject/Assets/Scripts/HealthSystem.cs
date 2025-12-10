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
    public float pulseSpeed = 6f;         // how fast it pulses
    public float pulseScaleAmount = 0.15f; // pulse height amount

    [Header("Health Text (Optional)")]
    public TextMeshProUGUI healthText;
    public TextMeshPro healthWorldText;

    [Header("Invincibility Frames")]
    public float invincibilityPeriod = 0.9f;
    public float preBlinkDelay = 0.1f;
    private bool isInvincible = false;
    public float blinkInterval = 0.07f;
    private SpriteRenderer[] spriteRenderers;

    void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Start()
    {
        // Initialize shared HP
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

        // Cache fill sprite
        healthBarSprite = healthBarFill.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        UpdateHealthBar();
    }


    // ---------------- DAMAGE ----------------
    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        sharedHealth = currentHealth;

        AudioManager.instance?.DamageTaken();

        // Flash red
        DamageFlash flash = GetComponent<DamageFlash>();
        if (flash != null)
            flash.Flash();

        UpdateAllHealthDisplays();

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
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


    // ---------------- DEATH ----------------
    IEnumerator Die()
    {
        AudioManager.instance?.Death();
        if (healthBarRoot != null)
            healthBarRoot.SetActive(false);

        gameObject.SetActive(false);
        GameManager.instance?.MarkPatientInfected();

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("ShootPatient");
    }


    // ---------------- HEALTH BAR ----------------
    void UpdateHealthBar()
    {
        if (healthBarRoot == null || healthBarFill == null)
            return;

        // show only when damaged
        healthBarRoot.SetActive(currentHealth < maxHealth);

        float ratio = (float)currentHealth / maxHealth;

        // -----------------------------------------
        // LOW HP EFFECTS ( < 25 percent )
        // -----------------------------------------
        if (ratio <= 0.25f)
        {
            // turn red
            if (healthBarSprite != null)
                healthBarSprite.color = Color.red;

            // pulse height (bar heartbeat)
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
            // NORMAL — no pulse, normal green
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


        // -----------------------------------------
        // WIDTH SMOOTH SHRINK
        // -----------------------------------------

        float currentX = healthBarFill.localScale.x;
        float targetX = ratio;  // full width is "1"

        float newX = Mathf.Lerp(currentX, targetX, Time.deltaTime * smoothSpeed);

        // Keep only width changing
        healthBarFill.localScale = new Vector3(
            newX,
            healthBarFill.localScale.y, // keep pulse height if low HP
            1f
        );
    }



    // ---------------- HEALTH TEXT ----------------
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
}
