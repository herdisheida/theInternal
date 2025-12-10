using UnityEngine;

public class BiteAttack : MonoBehaviour
{
    public int damage = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthSystem hs = collision.GetComponentInParent<HealthSystem>();

        if (hs != null)
        {
            hs.TakeDamage(damage);
        }
    }
}
