using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction shootAction;

    private Vector2 moveAmount;
    private float jumpAmount;
    private bool isGrounded;

    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    private Rigidbody2D rb;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;    // chỗ viên đạn bắn ra
    public float bulletSpeed = 8f;
    public float bulletDamage = 10f;
    public float bulletLifetime = 4f;

    private Vector2 facingDirection = Vector2.right;

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

    void Start()
    {
        moveAction = inputActions.FindAction("Move");
        jumpAction = inputActions.FindAction("Jump");
        shootAction = inputActions.FindAction("Shoot");

        moveAction.Enable();
        jumpAction.Enable();
        shootAction.Enable();

        shootAction.performed += ctx => Shoot();
    }

    void Update()
    {
        moveAmount = moveAction.ReadValue<Vector2>();
        jumpAmount = jumpAction.ReadValue<float>();

        // Cập nhật hướng player đang nhìn (trái hoặc phải)
        if (moveAmount.x > 0.1f)
            facingDirection = Vector2.right;
        else if (moveAmount.x < -0.1f)
            facingDirection = Vector2.left;

        rb.linearVelocity = new Vector2(moveAmount.x * moveSpeed, rb.linearVelocity.y);

        if (jumpAction.WasPressedThisFrame() && isGrounded)
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

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(facingDirection, bulletSpeed, bulletDamage, bulletLifetime, gameObject);
        }
    }
}
