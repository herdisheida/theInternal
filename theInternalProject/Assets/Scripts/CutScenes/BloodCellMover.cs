using UnityEngine;

public class BloodCellMover : MonoBehaviour
{
    public float speed = 3f;
    public float lifeTime = 6f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }
}
