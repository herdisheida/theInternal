using UnityEngine;
public class WolfBoomerangHitbox : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthSystem hs = collision.GetComponentInParent<HealthSystem>();
        if (hs != null)
            hs.TakeDamage(damage);
    }
}

