using UnityEngine;
using System.Collections;

public class TunnelMover : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float stopPos = 0f; // where tunnel should stop on screen

    public BackgroundScroller backgroundScroller; // reference to background scroller to stop it

    private bool notifiedPlayer = false;


    void Start()
    {

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

            // start nofifying player to auto move
            if (!notifiedPlayer)
            {
                notifiedPlayer = true;
                StartCoroutine(NotifyPlayerAfterDelay());
            }

            // stop moving tunnel
            enabled = false;
            return;
        }

        // move tunnel left
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }

    IEnumerator NotifyPlayerAfterDelay()
    {
        // wait 2 seconds so obstacles clear out
        yield return new WaitForSeconds(0.8f);

        AutoMoveToTunnel player = FindObjectOfType<AutoMoveToTunnel>();
        if (player != null) player.BeginAutoMove(transform);
    }

}
