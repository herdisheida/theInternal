using UnityEngine;
using System.Collections;

public class BatSwarm : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int damage = 1;
    public float lifeTime = 10f;
    public float randomWanderStrength = 1f;
    public float fadeDuration = 1f; // Duration of the fade-out

    private Transform player;
    private Vector3 randomDirection;
    private float changeDirectionTimer = 0.5f;
    private float changeDirectionInterval = 0.5f;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        Destroy(gameObject, lifeTime);
        randomDirection = Random.insideUnitCircle;

        StartCoroutine(FadeOutAfterLifetime());
    }

    void Update()
    {
        if (player == null) return;

        // Update random direction periodically
        changeDirectionTimer -= Time.deltaTime;
        if (changeDirectionTimer <= 0)
        {
            randomDirection = Random.insideUnitCircle;
            changeDirectionTimer = changeDirectionInterval;
        }

        // Move toward player with random wandering
        Vector3 targetDirection = (player.position - transform.position).normalized;
        Vector3 moveDirection = (targetDirection + randomDirection * randomWanderStrength).normalized;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<HealthSystem>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    private IEnumerator FadeOutAfterLifetime()
    {
        yield return new WaitForSeconds(lifeTime);

        float elapsedTime = 0f;
        Color startColor = spriteRenderer.color;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(startColor.a, 0, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
