using UnityEngine;

public class BossShooting : MonoBehaviour
{
    public float speed = 6f;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}