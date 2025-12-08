using UnityEngine;

public class BossController_Werewolf : MonoBehaviour
{
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float moveDistance = 2f;
    private Vector3 startPos;
    private float movementTime = 0f;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    public Transform healthBarFill;
    public GameObject healthBarRoot;
    public float smoothSpeed = 10f;

    private float fullBarWidth;
    private float fillHeight;
    private SpriteRenderer fillSprite;

    void Start()
    {
        startPos = transform.position;
        currentHealth = maxHealth;

        fullBarWidth = healthBarFill.localScale.x;
        fillHeight = healthBarFill.localScale.y;
        fillSprite = healthBarFill.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        MoveBoss();
        UpdateHealthBar();
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

    void UpdateHealthBar()
    {
        float ratio = (float)currentHealth / maxHealth;
        float newX = Mathf.Lerp(healthBarFill.localScale.x, fullBarWidth * ratio, Time.deltaTime * smoothSpeed);

        healthBarFill.localScale = new Vector3(newX, fillHeight, 1f);

        if (ratio <= 0.25f)
            fillSprite.color = Color.red;
        else
            fillSprite.color = Color.green;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        GetComponent<DamageFlash>()?.Flash();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
