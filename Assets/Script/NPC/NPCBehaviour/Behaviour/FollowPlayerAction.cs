using UnityEngine;

public class FollowPlayerAction : NPCAction
{
    private readonly float stoppingDistance;
    private readonly string moveSpeedKey;
    private readonly float gapCheckDistance;
    private readonly float groundCheckDepth;
    private readonly float obstacleCheckDistance;
    private readonly float jumpForce;
    private readonly float maxJumpHeight;
    private readonly LayerMask groundMask;
    private readonly LayerMask obstacleMask;
    private float coyoteTimer;
    private const float coyoteTime = 0.15f;

    public FollowPlayerAction(
        INPC npc,
        float stoppingDistance = 1.5f,
        string moveSpeedKey = "MoveSpeed",
        float jumpForce = 5.5f,
        float gapCheckDistance = 0.75f,
        float groundCheckDepth = 1.6f,
        float obstacleCheckDistance = 0.45f) : base(npc)
    {
        this.stoppingDistance = Mathf.Max(0.1f, stoppingDistance);
        this.moveSpeedKey = moveSpeedKey;
        this.jumpForce = Mathf.Max(2.5f, jumpForce);
        this.gapCheckDistance = Mathf.Max(0.2f, gapCheckDistance);
        this.groundCheckDepth = Mathf.Max(0.5f, groundCheckDepth);
        this.obstacleCheckDistance = Mathf.Max(0.2f, obstacleCheckDistance);

        groundMask = PlatformingNavigator.GroundMask;
        obstacleMask = PlatformingNavigator.ObstacleMask;

        float jumpHeight = 1.5f;
        if (npc?.Blackboard != null && npc.Blackboard.TryGet("MaxJumpHeight", out float storedHeight))
        {
            jumpHeight = Mathf.Max(0.75f, storedHeight);
        }
        maxJumpHeight = jumpHeight;
    }

    protected override NodeState DoAction()
    {
        if (npc.PlayerTransform == null)
        {
            return NodeState.Failure;
        }

        Vector3 current = npc.Transform.position;
        Vector3 target = npc.PlayerTransform.position;
        float distance = Vector3.Distance(current, target);

        if (distance <= stoppingDistance)
        {
            npc.Animator?.SetBool("IsMoving", false);
            return NodeState.Success;
        }

        float speed = npc.Blackboard != null && npc.Blackboard.TryGet(moveSpeedKey, out float storedSpeed)
            ? storedSpeed
            : (npc != null ? npc.MoveSpeed : 3f);

        Vector2 direction = (target - current).normalized;

        float horizontalSign = Mathf.Abs(direction.x) > 0.05f ? Mathf.Sign(direction.x) : 0f;
        Vector2 groundOrigin = PlatformingNavigator.GroundProbeOrigin(npc.Transform);
        Vector2 forwardOrigin = groundOrigin + Vector2.up * 0.05f;

        bool gapAhead = false;
        bool obstacleAhead = false;
        bool needsHeightAdjust = false;
        bool hasHeadroom = true;

        if (horizontalSign != 0f)
        {
            gapAhead = PlatformingNavigator.IsGapAhead(forwardOrigin, new Vector2(horizontalSign, 0f), gapCheckDistance, groundCheckDepth, groundMask);
            obstacleAhead = PlatformingNavigator.DetectObstacle(
                forwardOrigin + Vector2.up * 0.4f,
                new Vector2(horizontalSign, 0f),
                obstacleCheckDistance,
                obstacleMask,
                out _);
        }

        needsHeightAdjust = PlatformingNavigator.ShouldJumpForHeight(current, target, maxJumpHeight - 0.25f);
        if (needsHeightAdjust)
        {
            hasHeadroom = PlatformingNavigator.HasHeadroom(forwardOrigin, maxJumpHeight + 0.5f, obstacleMask);
        }

        if (npc.IsGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        bool shouldJump = (needsHeightAdjust && hasHeadroom) || obstacleAhead || gapAhead;
        if (shouldJump && coyoteTimer <= 0f)
        {
            shouldJump = false;
        }

        // perform movement using Rigidbody if available to respect physics
        if (npc?.Rigidbody != null)
        {
            // horizontal movement preserving vertical velocity
            float desiredVelX = direction.x * speed;
            if (gapAhead && !shouldJump)
            {
                desiredVelX = 0f;
            }
            npc.Move(new Vector2(desiredVelX, 0f));

            if (shouldJump && (npc.IsGrounded || coyoteTimer > 0f))
            {
                npc.Jump(jumpForce);
                coyoteTimer = 0f;
            }

            npc.Animator?.SetBool("IsMoving", Mathf.Abs(npc.Rigidbody.linearVelocity.x) > 0.1f);
        }
        else
        {
            Vector3 next = Vector3.MoveTowards(current, target, speed * Time.deltaTime);
            if (gapAhead && !shouldJump)
            {
                next.x = current.x;
            }
            if (shouldJump)
            {
                next.y = Mathf.Max(next.y, current.y + (jumpForce * 0.2f * Time.deltaTime));
            }
            npc.Transform.position = next;
            npc.Animator?.SetBool("IsMoving", true);
        }

        npc.Blackboard?.Set("FollowGapAhead", gapAhead);
        npc.Blackboard?.Set("FollowObstacleAhead", obstacleAhead);
        npc.Blackboard?.Set("FollowNeedsJump", shouldJump);
        npc.Blackboard?.Set("FollowHeightDelta", target.y - current.y);

        return NodeState.Running;
    }
}
