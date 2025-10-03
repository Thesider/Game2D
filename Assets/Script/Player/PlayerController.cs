using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    [Header("Player Component Referrences")]
    [SerializeField] private Rigidbody2D rb;
    [Header("Player Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private float horizontalInput;

    [SerializeField] private SpriteRenderer characterSR;
    private Vector2 moveInput;

    private void FixedUpdate() {
        // Handle horizontal movement
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        // Ground check
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        if (isGrounded && Input.GetKeyDown(KeyCode.Space)) {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }
    #region Player_Controls
    public void Move(InputAction.CallbackContext context) {
        horizontalInput = context.ReadValue<Vector2>().x;

        if( moveInput.x != 0 ) {
            if (moveInput.x > 0) characterSR.transform.localScale = new Vector2(1, 1);
            else characterSR.transform.localScale = new Vector2(-1, 1);

        }
    }

    public void Jump(InputAction.CallbackContext context) {
        if (context.performed && isGrounded()) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private bool isGrounded() {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1f, 0.1f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }
    #endregion



}
