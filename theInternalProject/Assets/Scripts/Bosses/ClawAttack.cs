using UnityEngine;

public class ClawAttack : MonoBehaviour
{

    
    [Header("Damage Settings")]
    public int damage = 20;

    [Header("Attack Sprite Effects")]
    public Sprite idleSprite;       // normal hand/claw sprite
    public Sprite attackSprite;     // slashing / claw extended sprite
    public float attackSpriteTime = 0.15f; // how long to show attack sprite

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (sr != null && idleSprite != null)
            sr.sprite = idleSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Deal damage
        HealthSystem hs = collision.GetComponentInParent<HealthSystem>();
        if (hs != null)
        {
            hs.TakeDamage(damage);
        }

        // Play sprite animation if valid
        if (sr != null && attackSprite != null)
            StartCoroutine(PlayClawAnimation());
    }

    private System.Collections.IEnumerator PlayClawAnimation()
    {
        // Switch to claw attack sprite
        sr.sprite = attackSprite;

        // Wait a little bit
        yield return new WaitForSeconds(attackSpriteTime);

        // Return to idle sprite
        if (idleSprite != null)
            sr.sprite = idleSprite;
    }
}

