using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [Header("Segments (children you want to loop)")]
    public Transform[] segments;

    [Header("Speed")]
    public ObstacleSpawner obstacleSpawner; // reference to your existing spawner
    public float speedMultiplier = 1f;      // 1 = same speed as obstacles

    [Header("Control")]
    public bool isScrolling = true;

    private float segmentWidth;

    void Start()
    {
        if (segments == null || segments.Length == 0)
        {
            // auto-fill from children if not set
            int childCount = transform.childCount;
            segments = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
                segments[i] = transform.GetChild(i);
        }

        // assume all segments same width, use first one
        var sr = segments[0].GetComponent<SpriteRenderer>();
        if (sr != null)
            segmentWidth = sr.bounds.size.x;
        else
            Debug.LogError("Background segment needs a SpriteRenderer!");
    }

    void Update()
    {
        if (!isScrolling || segments == null || segments.Length == 0)
            return;

        float speed = GetCurrentSpeed();
        Vector3 move = Vector3.left * speed * Time.deltaTime;

        // move all segments left
        foreach (Transform t in segments)
        {
            t.position += move;
        }

        // loop segments
        // find rightmost segment so we know where to place wrapped ones
        Transform rightmost = segments[0];
        foreach (Transform t in segments)
        {
            if (t.position.x > rightmost.position.x)
                rightmost = t;
        }

        foreach (Transform t in segments)
        {
            // if this segment has gone fully off-screen to the left, move it to the right
            if (t.position.x <= rightmost.position.x - segmentWidth * (segments.Length - 1))
            {
                t.position = new Vector3(
                    rightmost.position.x + segmentWidth,
                    t.position.y,
                    t.position.z
                );
                rightmost = t; // update rightmost
            }
        }
    }

    float GetCurrentSpeed()
    {
        if (obstacleSpawner != null)
            return obstacleSpawner.obstacleMoveSpeed * speedMultiplier;

        // if no spawner assigned, just choose a default
        return 3f;
    }

    public void StopScrolling()
    {
        isScrolling = false;
    }
}
