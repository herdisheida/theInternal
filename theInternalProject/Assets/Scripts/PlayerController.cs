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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveInputX = Input.GetAxisRaw("Horizontal");
        float moveInputY = Input.GetAxisRaw("Vertical");
        
        moveInput = new Vector2(moveInputX, moveInputY).normalized;

    }

    void FixedUpdate()
    {
        Vector2 targetPosition = rb.position + moveSpeed * moveInput * Time.fixedDeltaTime;

        // top & bottom bounds
        if (topBorder != null && bottomBorder != null)
        {
            float maxY = topBorder.bounds.min.y - padding;
            float minY = bottomBorder.bounds.max.y + padding;

            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        // left & right bounds
        if (leftBorder != null && rightBorder != null)
        {
            float minX = leftBorder.bounds.max.x + padding;
            float maxX = rightBorder.bounds.min.x - padding;

            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        }

        rb.MovePosition(targetPosition);
    }

    
}
