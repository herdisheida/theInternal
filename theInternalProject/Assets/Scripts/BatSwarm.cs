using UnityEngine;
using System.Collections;

public class BatSwarm : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int damage = 1;
    public float lifeTime = 4f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (player == null) return;

        // Move toward player, but ONLY the bat swarm object
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<HealthSystem>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
