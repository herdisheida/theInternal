using UnityEngine;
using System.Collections;

public class ClawAttack : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 20;

    [Header("Scene Start Delay")]
    public float sceneStartDelay = 1.4f;

    [Header("Attack Sprite Effects")]
    public Sprite idleSprite;
    public Sprite attackSprite;
    public float attackSpriteTime = 0.15f;

    private SpriteRenderer sr;
    private bool canDamage = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (sr != null && idleSprite != null)
            sr.sprite = idleSprite;

        StartCoroutine(EnableDamageAfterDelay());
    }

    IEnumerator EnableDamageAfterDelay()
    {
        yield return new WaitForSeconds(sceneStartDelay);
        canDamage = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canDamage)
            return;

        HealthSystem hs = collision.GetComponentInParent<HealthSystem>();
        if (hs != null)
            hs.TakeDamage(damage);

        if (sr != null && attackSprite != null)
            StartCoroutine(PlayClawAnimation());
    }

    private IEnumerator PlayClawAnimation()
    {
        sr.sprite = attackSprite;
        yield return new WaitForSeconds(attackSpriteTime);

        if (idleSprite != null)
            sr.sprite = idleSprite;
    }
}
