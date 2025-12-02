using UnityEngine;

public class TunnelMover : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float stopPos = 0f; // where tunnel should stop on screen

    void Start()
    {
        
    }

    void Update()
    {
        if (transform.position.x <= stopPos)
        {
            transform.position = new Vector3(stopPos, transform.position.y, transform.position.z);
            enabled = false; // stop moving
            return;
        }
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }
}
