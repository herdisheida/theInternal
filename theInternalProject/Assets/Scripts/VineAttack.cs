using UnityEngine;
using System.Collections;

public class VineAttack : MonoBehaviour
{
    public int damage = 10;

    [Header("Timing")]
    public float warningTime = 2f;   // how long vine "prepares"
    public float stretchSpeed = 4f;    // how fast it extends
    public float retractSpeed = 3f;    // how fast it retracts

    private SpriteRenderer sr;
    private Vector3 baseScale;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;

        // Start vine completely hidden (scale = 0)
        transform.localScale = new Vector3(baseScale.x, 0f, baseScale.z);

        StartCoroutine(VineRoutine());
    }

    IEnumerator VineRoutine()
    {
        //  1. WARNING — wait before stretching
        yield return new WaitForSeconds(warningTime);

        //  2. STRETCH DOWNWARD
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * stretchSpeed;

            // Scale from 0 to full height
            float stretch = Mathf.Lerp(0f, baseScale.y, t);
            transform.localScale = new Vector3(baseScale.x, stretch, baseScale.z);

            yield return null;
        }

        // leave fully extended for a moment
        yield return new WaitForSeconds(0.1f);

        //  3. RETRACT UPWARD
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * retractSpeed;
            float shrink = Mathf.Lerp(baseScale.y, 0f, t);
            transform.localScale = new Vector3(baseScale.x, shrink, baseScale.z);

            yield return null;
        }

        Destroy(gameObject);
    }

    //  DAMAGE USING COLLIDER
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HealthSystem hs = collision.GetComponentInParent<HealthSystem>();
            if (hs != null)
                hs.TakeDamage(damage);
        }
    }
}
