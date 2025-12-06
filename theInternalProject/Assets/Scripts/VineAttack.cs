using UnityEngine;
using System.Collections;

public class VineAttack : MonoBehaviour
{
    public int damage = 10;

    [Header("Timing")]
    public float warningTime = 2f;
    public float stretchSpeed = 4f;
    public float retractSpeed = 3f;

    [Header("Warning Indicator")]
    public GameObject warningIndicatorPrefab;   // Assign in Inspector

    // Offset to control *where* the warning appears relative to vine
    public Vector3 indicatorOffset = new Vector3(0, -6f, 0);

    private GameObject activeIndicator;

    private SpriteRenderer sr;
    private Vector3 baseScale;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;

        // Start vine hidden
        transform.localScale = new Vector3(baseScale.x, 0f, baseScale.z);

        StartCoroutine(VineRoutine());
    }

    IEnumerator VineRoutine()
    {
        // The vine actually appears at peekPos, not at original transform.position
        Vector3 startPos = transform.position;
        Vector3 peekPos = startPos - new Vector3(0, 1f, 0);

        // Spawn the warning where the vine will be seen
        Vector3 attackPosition = peekPos + indicatorOffset;

        if (warningIndicatorPrefab != null)
        {
            activeIndicator = Instantiate(warningIndicatorPrefab, attackPosition, Quaternion.identity);

            SpriteRenderer indSR = activeIndicator.GetComponent<SpriteRenderer>();
            if (indSR != null)
            {
                indSR.enabled = true;
                indSR.sortingOrder = 999;
            }

            activeIndicator.transform.localScale = Vector3.one;
        }

        Debug.Log("Indicator spawned at: " + attackPosition);

        // 2. Peek vine slightly
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            transform.position = Vector3.Lerp(startPos, peekPos, t);
            yield return null;
        }


        // 3. Wait during warning time
        yield return new WaitForSeconds(warningTime);

        // Remove indicator right before the vine strikes
        if (activeIndicator != null)
            Destroy(activeIndicator);

        // 4. Stretch downward
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

        // 5. Retract
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
