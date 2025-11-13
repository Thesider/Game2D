using UnityEngine;

public class Movement : CoreComponent {
    public Rigidbody2D rb { get; private set; }

    public int facingDirection { get; private set; }

    public Vector2 currentVelocity { get; private set; }

    private Vector2 workspace;

    protected override void Awake() {
        base.Awake();

        rb = GetComponentInParent<Rigidbody2D>();

        facingDirection = 1;
    }

    public void LogicUpdate() {
        currentVelocity = rb.linearVelocity;
    }

    #region Set Functions
    public void SetVelocityZero() {
        rb.linearVelocity = Vector2.zero;
        currentVelocity = Vector2.zero;
    }
    public void SetVelocity(float velocity, Vector2 angle, int direction) {
        angle.Normalize();
        workspace.Set(angle.x * velocity * direction, angle.y * velocity);
        rb.linearVelocity = workspace;
        currentVelocity = workspace;
    }

    public void SetVelocity(float velocity, Vector2 direction) {
        workspace = direction * velocity;
        rb.linearVelocity = workspace;
        currentVelocity = workspace;
    }
    public void SetVelocityX(float velocity) {
        workspace.Set(velocity, currentVelocity.y);
        rb.linearVelocity = workspace;
        currentVelocity = workspace;
    }

    public void SetVelocityY(float velocity) {
        workspace.Set(currentVelocity.x, velocity);
        rb.linearVelocity = workspace;
        currentVelocity = workspace;
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
