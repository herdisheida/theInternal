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
    public float padding = 0.3f;           // avoid player being inside the border

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

        // fix movement within bounds
        if (topBorder != null && bottomBorder != null)
        {
            float maxY = topBorder.bounds.min.y - padding;
            float minY = bottomBorder.bounds.max.y + padding;

            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        rb.MovePosition(targetPosition);
    }

    
}
