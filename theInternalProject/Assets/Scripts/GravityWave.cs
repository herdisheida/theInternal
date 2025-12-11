using UnityEngine;

public class GravityWave : MonoBehaviour
{
    public float startRadius = 0.2f;
    public float maxRadius = 4f;
    public float expandSpeed = 5f;
    public int damage = 1;

    private CircleCollider2D col;
    private Vector3 startScale;

    void Start()
    {
        col = GetComponent<CircleCollider2D>();
        startScale = transform.localScale;

        // Start tiny
        transform.localScale = startScale * startRadius;
    }

    void Update()
    {
        // Expand the wave
        float scale = transform.localScale.x;
        scale += expandSpeed * Time.deltaTime;

        transform.localScale = new Vector3(scale, scale, 1f);

        // Update collider radius (scaled)
        if (col != null)
            col.radius = scale * 0.5f;

        // Destroy when max size reached
        if (scale >= maxRadius)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.CompareTag("Player"))
        {
            var hp = c.GetComponentInParent<HealthSystem>();
            hp?.TakeDamage(damage);
        }
    }
}
