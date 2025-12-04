using UnityEngine;

// Infection mechanics in Obsticle Gameplay
public class InfectionController : MonoBehaviour
{
    [Header("Damage to player on touch")]
    private int damagePlayerAmount = 15; // how much health to remove from player per hit

    [Header("Infection health settings")]
    public int maxHealth = 4;          // 3–4 shots KO
    public float hitFlashDuration = 0.1f;

    private int currentHealth;
    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;
    }

    void Update()
    {
        
    }

    // PLAYER TOUCH - damage the player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthSystem hs = other.GetComponent<HealthSystem>();
            if (hs != null)
            {
                hs.TakeDamage(damagePlayerAmount);
            }
        }
    }

    // BULLET HIT: player shoots infection
    public void TakeBulletDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth > 0)
        {
            // infection still alive => flash red
            if (sr != null)
                StartCoroutine(HitFlash());
        }
        else
        {
            // infection dead => destroy infection
            Destroy(gameObject);
        }
    }

    private System.Collections.IEnumerator HitFlash()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(hitFlashDuration);
        sr.color = originalColor;
    }
}
