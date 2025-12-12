using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

// Player movement and interaction in Obsticle Gameplay and Boss Fight
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 7.5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("Movement Bounds")]
    public BoxCollider2D topBorder;
    public BoxCollider2D bottomBorder;
    public BoxCollider2D leftBorder;
    public BoxCollider2D rightBorder;
    public float padding = 0.7f; // avoid player being inside the border

    [Header("Lean / Tilt")]
    public float maxLeanAngle = 10f;
    public float leanSpeed = 8f;
    private float currentLean;
    private float leanVelocity;
    
    [Header("Gun")]
    public Transform gunTransform; 




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveInputX = Input.GetAxisRaw("Horizontal");
        float moveInputY = Input.GetAxisRaw("Vertical");

        HealthSystem hp = GetComponent<HealthSystem>();
        if (hp != null && hp.IsStunned)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = new Vector2(moveInputX, moveInputY).normalized;

        // lean calculation
        float targetLean = -moveInput.x * maxLeanAngle;
        currentLean = Mathf.SmoothDamp(
            currentLean,
            targetLean,
            ref leanVelocity,
            0.1f
        );

        // apply lean to player
        transform.rotation = Quaternion.Euler(0f, 0f, currentLean);
    }

    void FixedUpdate()
    {
        // 1, Get external force
        HealthSystem hp = GetComponent<HealthSystem>();
        Vector2 extraForce = hp != null ? hp.ConsumeExternalForce() : Vector2.zero;

        // 2, Base movement
        Vector2 movement = moveSpeed * moveInput * Time.fixedDeltaTime;

        // 3, Apply both
        Vector2 targetPosition = rb.position + movement + extraForce * Time.fixedDeltaTime;

        // Bounds
        if (topBorder != null && bottomBorder != null)
        {
            float maxY = topBorder.bounds.min.y - padding;
            float minY = bottomBorder.bounds.max.y + padding;
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        if (leftBorder != null && rightBorder != null)
        {
            float minX = leftBorder.bounds.max.x + padding;
            float maxX = rightBorder.bounds.min.x - padding;
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        }

        rb.MovePosition(targetPosition);
    }

    void LateUpdate()
    {
        // force gun to stay upright and shoot straight
        if (gunTransform != null)
        {
            gunTransform.rotation = Quaternion.identity;
        }
    }
    
}
