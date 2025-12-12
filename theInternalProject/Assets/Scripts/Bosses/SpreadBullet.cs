using UnityEngine;

public class SpreadBullet : MonoBehaviour
{
    public float speed = 6f;
    public float lifetime = 2f;
    private Vector2 direction;

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
}
