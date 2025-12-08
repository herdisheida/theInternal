using UnityEngine;

// automove player toward tunnel when activated
public class AutoMoveToTunnel : MonoBehaviour
{
    [Header("Auto Move Settings")]
    public float horizontalSpeed = 4f;
    public float stopOffsetX = 0.3f;

    private Transform tunnel;
    private bool autoMoving = false;

    void Start()
    {
        
    }

    // when the tunnel has spawned and should pull the player in
    public void BeginAutoMove(Transform tunnelTransform)
    {
        tunnel = tunnelTransform;
        autoMoving = true;
        Debug.Log("AutoMoveToTunnel: Begin auto move toward tunnel.");
    }

    void Update()
    {
        if (!autoMoving || tunnel == null) return;

        Vector3 pos = transform.position;

        // where we want the player to stop (a bit before the tunnel)
        float targetX = tunnel.position.x - stopOffsetX;

        // only move if we're still left of that point
        if (pos.x < targetX)
        {
            pos.x += horizontalSpeed * Time.deltaTime;
            transform.position = pos;
        }
        else
        {
            autoMoving = false;   // reached tunnel entrance
            Debug.Log("AutoMoveToTunnel: Reached tunnel.");
        }
    }
}
