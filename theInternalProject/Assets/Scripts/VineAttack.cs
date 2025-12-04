using UnityEngine;
using System.Collections;

public class VineAttack : MonoBehaviour
{
    public int damage = 10;

    [Header("Timing")]
    public float warningTime = 4f;   // how long vine "prepares"
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
        // 1. Peek (vine moves down slightly)
        Vector3 peekPos = transform.position - new Vector3(0, 0.5f, 0);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;  // peek speed
            transform.position = Vector3.Lerp(transform.position, peekPos, t);
            yield return null;
        }

        // 2. Hold (warning)
        yield return new WaitForSeconds(warningTime);

        // 3. Stretch downward
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * stretchSpeed;
            transform.localScale = new Vector3(
                baseScale.x,
                Mathf.Lerp(0f, baseScale.y, t),
                baseScale.z
            );
            yield return null;
        }

        // 4. Retract
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * retractSpeed;
            transform.localScale = new Vector3(
                baseScale.x,
                Mathf.Lerp(baseScale.y, 0f, t),
                baseScale.z
            );
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
