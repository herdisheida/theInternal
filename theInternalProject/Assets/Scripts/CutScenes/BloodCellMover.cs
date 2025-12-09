using UnityEngine;

public class BloodCellMover : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;      // how fast they slide left
    public float lifetime = 6f;       // how long before auto-destroy

    [Header("Rotation")]
    public float minRotSpeed = 15f;   // degrees per second
    public float maxRotSpeed = 60f;

    private float rotationSpeed;      // signed (left/right)

    void Start()
    {
        // random direction: +1 (clockwise) or -1 (counter-clockwise)
        float sign = Random.value > 0.5f ? 1f : -1f;
        float magnitude = Random.Range(minRotSpeed, maxRotSpeed);
        rotationSpeed = sign * magnitude;

        // clean up after a while
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // move left across the screen
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        // rotate slowly
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
