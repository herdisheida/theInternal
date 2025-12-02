using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

// Player movement and interaction in Obsticle Gameplay and Boss Fight
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

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
        rb.MovePosition(targetPosition);
    }

    
}
