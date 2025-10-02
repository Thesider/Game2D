using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllera : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction jumpAction;

    private Vector2 moveAmount;
    private float jumpAmount;

    private bool isGrounded;

    public float moveSpeed = 5f;
    public float jumpForce = 5f;


    private Rigidbody2D rb;

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();

    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = inputActions.FindAction("Move");
        jumpAction = inputActions.FindAction("Jump");

        moveAction.Enable();
        jumpAction.Enable();
    }
    // Update is called once per frame
    void Update()
    {
        moveAmount = moveAction.ReadValue<Vector2>();
        jumpAmount = jumpAction.ReadValue<float>();
        rb.linearVelocity = new Vector2(moveAmount.x * moveSpeed, rb.linearVelocity.y);

        if (jumpAction.WasPressedThisFrame() && isGrounded == true)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        Walk();
    }
    private void Walk()
    {
        rb.linearVelocity = new Vector2(moveAmount.x * moveSpeed, rb.linearVelocity.y);

    }
    private void Jump()
    {
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        isGrounded = false;



    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

}




