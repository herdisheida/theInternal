using UnityEngine;

public class ClawProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 20;
    public float lifetime = 2f;
    public Vector2 direction = Vector2.right;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthSystem hs = collision.GetComponentInParent<HealthSystem>();

        if (hs != null)
        {
            hs.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
