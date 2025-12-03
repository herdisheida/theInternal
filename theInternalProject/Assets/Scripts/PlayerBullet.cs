using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TRIGGER with: " + collision.gameObject.name);
    }

    
}