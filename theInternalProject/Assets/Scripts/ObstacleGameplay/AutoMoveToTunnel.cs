using UnityEngine;

// automove player toward tunnel when activated
public class AutoMoveToTunnel : MonoBehaviour
{
    [Header("Auto Move Settings")]
    public float moveSpeed = 4f;

    // where you want the player to end up
    public Vector3 targetPosition = new Vector3(5.5f, 0f, 0f);

    private bool autoMoving = false;

    void Start()
    {
        
    }

    // when the tunnel has spawned and should pull the player in
    public void BeginAutoMove(Transform tunnelTransform)
    {
        autoMoving = true;
    }

    void Update()
    {
        if (!autoMoving) return;

        // move diagonally toward the final target (center)
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // stop when close enough
        if (Vector3.Distance(transform.position, targetPosition) < 0.02f)
        {
            autoMoving = false;
        }
    }
}
