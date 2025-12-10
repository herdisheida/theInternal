using UnityEngine;

public class WindSlash : MonoBehaviour
{
    private Vector2 velocity;
    public float lifeTime = 3f;
    public int damage = 1;

    public void SetVelocity(Vector2 vel)
    {
        velocity = vel;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Only move THIS projectile, not the boss
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Damage
            var hp = collision.GetComponentInParent<HealthSystem>();
            if (hp != null)
                hp.TakeDamage(damage);

            // Knockback
            Rigidbody2D rb = collision.GetComponentInParent<Rigidbody2D>();
            if (rb != null)
            {
                float direction = Mathf.Sign(transform.position.x - collision.transform.position.x);
                Vector2 knockForce = new Vector2(-direction * 10f, 3f); 
                rb.AddForce(knockForce, ForceMode2D.Impulse);
            }

            Destroy(gameObject);
        }
    }

}
