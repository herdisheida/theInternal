using UnityEngine;

public class TunnelMover : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float stopPos = 0f; // where tunnel should stop on screen

    public BackgroundScroller backgroundScroller; // reference to background scroller to stop it

    void Start()
    {
        // auto find background scroller
        if (backgroundScroller == null)
        {
            backgroundScroller = FindObjectOfType<BackgroundScroller>();
        }
    }

    void Update()
    {
        if (transform.position.x <= stopPos)
        {
            // snap into correct pos
            transform.position = new Vector3(stopPos, transform.position.y, transform.position.z);

            // stop background scrolling
            if (backgroundScroller != null)
            {
                backgroundScroller.StopScrolling();
            }

            // stop moving tunnel
            enabled = false;
            return;
        }

        // move tunnel left
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

    }
}
