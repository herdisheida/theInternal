using UnityEngine;

public class PlayerBullet : MonoBehaviour
{

    public int damage = 1;
    public float speed = 15f;
    public float lifetime = 2f;

    private Vector2 direction = Vector2.right; // default

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        // hit boss
        if (collision.CompareTag("Boss"))
        {
            // Try Zombie Boss
            BossController zombieBoss = collision.GetComponent<BossController>();
            if (zombieBoss != null)
            {
                zombieBoss.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            // Try Werewolf Boss
            BossController_Werewolf wolfBoss = collision.GetComponent<BossController_Werewolf>();
            if (wolfBoss != null)
            {
                wolfBoss.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            Vampire vampireBoss = collision.GetComponent<Vampire>();
            if (vampireBoss != null)
            {
                vampireBoss.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            Destroy(gameObject);
        }

    }
}
