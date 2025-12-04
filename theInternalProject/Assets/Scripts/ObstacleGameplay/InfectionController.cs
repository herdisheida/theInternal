using UnityEngine;

// Infection mechanics in Obsticle Gameplay
public class InfectionController : MonoBehaviour
{
    [Header("Damage to player on touch")]
    private int damagePlayerAmount = 15; // how much health to remove from player per hit

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthSystem hs = other.GetComponent<HealthSystem>();
            if (hs != null)
            {
                hs.TakeDamage(damagePlayerAmount);
            }
        }
    }
}
