using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset inputActions;
    public PlayerData playerData;
    public DeadScreen deadScreen;

    private InputActionMap playerActionMap;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction shootAction;

    private Vector2 moveAmount;
    private float jumpAmount;
    private bool isGrounded;

    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    

    public HealthBarSlider healthBar;
    private Rigidbody2D rb;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;
    public float bulletDamage = 10f;
    public float bulletLifetime = 4f;

    private Vector2 facingDirection = Vector2.right;

    private void Awake()
    {
        SaveSystem.Load(playerData);

        rb = GetComponent<Rigidbody2D>();

        healthBar.SetMaxHealth(playerData.maxHealth);
        healthBar.SetHealth(playerData.currentHealth);

        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset reference missing on PlayerController.", this);
            return;
        }

        playerActionMap = inputActions.FindActionMap("Player", throwIfNotFound: false);
        if (playerActionMap == null)
        {
            Debug.LogError("'Player' action map not found in InputActionAsset.", this);
            return;
        }

        moveAction = playerActionMap.FindAction("Move", throwIfNotFound: false);
        jumpAction = playerActionMap.FindAction("Jump", throwIfNotFound: false);
        shootAction = playerActionMap.FindAction("Attack", throwIfNotFound: false);

        if (moveAction == null || jumpAction == null || shootAction == null)
        {
            Debug.LogError("One or more required actions (Move, Jump, Attack) are missing from the 'Player' action map.", this);
        }
    }

    private void OnEnable()
    {
        if (playerActionMap == null)
            return;

        playerActionMap.Enable();
        moveAction?.Enable();
        jumpAction?.Enable();
        shootAction?.Enable();

        if (shootAction != null)
        {
            shootAction.performed += OnShootPerformed;
        }
    }
   

    private void OnDisable()
    {
        if (playerActionMap == null)
            return;

        if (shootAction != null)
        {
            shootAction.performed -= OnShootPerformed;
            shootAction.Disable();
        }

        jumpAction?.Disable();
        moveAction?.Disable();
        playerActionMap.Disable();
    }

    void Update()
    {
        if (moveAction == null || jumpAction == null)
            return;

        moveAmount = moveAction.ReadValue<Vector2>();
        jumpAmount = jumpAction.ReadValue<float>();

        if (moveAmount.x > 0.1f)
            facingDirection = Vector2.right;
        else if (moveAmount.x < -0.1f)
            facingDirection = Vector2.left;

        if (rb == null)
            return;

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
        if (rb == null)
            return;

        rb.linearVelocity = new Vector2(moveAmount.x * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        if (rb == null)
            return;

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

    void TakeDamage(int damage)
    {
        playerData.currentHealth -= damage;
        healthBar.SetHealth(playerData.currentHealth);
        SaveSystem.Save(playerData); // optional autosave

        if (playerData.currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (deadScreen != null)
        {
            deadScreen.Show();
        }
        // disable player movement, shooting, etc.
        this.enabled = false;
    }

    private void OnShootPerformed(InputAction.CallbackContext context)
    {
        Shoot();
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        BulletPool.Spawn(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity,
            facingDirection,
            bulletSpeed,
            bulletDamage,
            bulletLifetime,
            gameObject);
    }
}
