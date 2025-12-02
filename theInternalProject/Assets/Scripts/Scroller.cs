using UnityEngine;

public class Scroller : MonoBehaviour
{
    public float moveSpeed = 5f;   // how fast obstacles move left
    public float destroyPos = -15f;  // pos where we delete the obstacles

    void Start()
    {
        
    }

    void Update()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        if (transform.position.x < destroyPos)
        {
            Destroy(gameObject);
        }
    }
}
