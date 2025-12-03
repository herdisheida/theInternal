using UnityEngine;

// Infection mechanics in Obsticle Gameplay
public class InfectionController : MonoBehaviour
{
    [SerializeField] private int damagePlayerAmount = 10; // how much health to remove per hit

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
