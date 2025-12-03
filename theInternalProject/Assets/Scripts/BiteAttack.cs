using UnityEngine;

public class BiteAttack : MonoBehaviour
{
    public int damage = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("BITE HIT: " + collision.name);

        HealthSystem hs = collision.GetComponentInParent<HealthSystem>();

        if (hs != null)
        {
            Debug.Log("Player took damage!");
            hs.TakeDamage(damage);
        }
    }
}
