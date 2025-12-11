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
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var hp = collision.GetComponentInParent<HealthSystem>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
                
                float direction = Mathf.Sign(transform.position.x - collision.transform.position.x);
                hp.ApplyKnockback(new Vector2(-direction * 10f, 3f));
            }

            Destroy(gameObject);
        }
    }

}
