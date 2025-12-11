using UnityEngine;

public class WindSlash : MonoBehaviour
{
    private Vector2 velocity;
    public float lifeTime = 3f;
    public int damage = 1;
    public float stunDuration = 0.4f;   // how long the player is stunned

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
        if (!collision.CompareTag("Player")) return;

        var hp = collision.GetComponentInParent<HealthSystem>();
        var pc = collision.GetComponentInParent<PlayerController>();

        if (hp != null)
        {
            hp.TakeDamage(damage);

            // Knockback
            Rigidbody2D rb = collision.GetComponentInParent<Rigidbody2D>();
            if (rb)
            {
                float direction = Mathf.Sign(transform.position.x - collision.transform.position.x);
                float y = Random.Range(-0.5f, 0.5f);
                hp.ApplyKnockback(new Vector2(-direction * 7f, y));

            }
        }

        // Apply stun
        if (pc != null)
        {
            pc.StartCoroutine(StunPlayer(pc, stunDuration));
        }

        Destroy(gameObject);
    }

    // stunning coroutine
    private System.Collections.IEnumerator StunPlayer(PlayerController pc, float duration)
    {
        pc.enabled = false; // disable movement

        // OPTIONAL: quick hit flash
        SpriteRenderer sr = pc.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            Color orig = sr.color;
            sr.color = Color.white;
            yield return new WaitForSeconds(0.05f);
            sr.color = orig;
        }

        yield return new WaitForSeconds(duration);

        pc.enabled = true; // restore control
    }
}
