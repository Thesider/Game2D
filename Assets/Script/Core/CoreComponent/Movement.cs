using UnityEngine;

public class Movement : CoreComponent {
    public Rigidbody2D rb { get; private set; } 
    public int facingDirection { get; private set; }
    public bool canSetVelocity { get; set; }

    public Vector2 currentVelocity { get; private set; }

    private Vector2 workspace;

    protected override void Awake() {
        base.Awake();

        rb = GetComponentInParent<Rigidbody2D>();

        facingDirection = 1;
        canSetVelocity = true;
    }

    public void LogicUpdate() {
        currentVelocity = rb.linearVelocity;
    }

    #region Set Functions
    public void SetVelocityZero() {
        workspace = Vector2.zero;
        SetFinalVelocity();

    }
    public void SetVelocity(float velocity, Vector2 angle, int direction) {
        angle.Normalize();
        workspace.Set(angle.x * velocity * direction, angle.y * velocity);
        SetFinalVelocity();

    }

    public void SetVelocity(float velocity, Vector2 direction) {
        workspace = direction * velocity;
        SetFinalVelocity();

    }
    public void SetVelocityX(float velocity) {
        workspace.Set(velocity, currentVelocity.y);
        SetFinalVelocity();
    }

    public void SetVelocityY(float velocity) {
        workspace.Set(currentVelocity.x, velocity);
        SetFinalVelocity();
    }
    private void SetFinalVelocity() {
        if (canSetVelocity) {
            rb.linearVelocity = workspace;
            currentVelocity = workspace;
        }
    }
    private void Flip() {
        facingDirection *= -1;
        rb.transform.Rotate(0.0f, 180.0f, 0.0f);

    }
    public void CheckFlip(int xInput) {
        if (xInput != 0 && xInput != facingDirection) {
            Flip();
        }
    }
    #endregion
}
