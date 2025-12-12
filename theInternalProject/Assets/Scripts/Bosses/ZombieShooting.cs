using UnityEngine;

public class BossShooting : MonoBehaviour
{
    public float speed = 6f;
    public float lifetime = 3f;
    public int damage = 10;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if bullet hits the player
        if (collision.CompareTag("Player"))
        {
            HealthSystem health = collision.GetComponent<HealthSystem>();

            if (health != null)
            {
                health.TakeDamage(damage);
            }

            Destroy(gameObject); // Bullet disappears on hit
        }
    }
}
