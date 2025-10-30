using UnityEngine;

// Move towards the player until within attack range.
public class MoveToPlayerAction : Action
{
    private readonly bool debug;
<<<<<<< HEAD
=======
    private readonly Rigidbody2D rb;
    private readonly LayerMask groundMask;
    private readonly LayerMask obstacleMask;
    private readonly float gapCheckDistance = 0.65f;
    private readonly float groundCheckDepth = 1.4f;
    private readonly float obstacleCheckDistance = 0.45f;
    private readonly float jumpForce = 6f;
    private readonly float jumpCooldown = 0.35f;
    private float lastJumpTime = -5f;
>>>>>>> main

    public MoveToPlayerAction(IEnemy enemy, bool debug = false) : base(enemy)
    {
        this.debug = debug;
<<<<<<< HEAD
=======
        if (enemy is MonoBehaviour mb)
        {
            rb = mb.GetComponent<Rigidbody2D>();
        }

        groundMask = PlatformingNavigator.GroundMask;
        obstacleMask = PlatformingNavigator.ObstacleMask;
>>>>>>> main
    }

    protected override NodeState DoAction()
    {
        if (enemy.Player == null) return NodeState.Failure;

<<<<<<< HEAD
        Vector3 target = enemy.Player.position;
        float dist = Vector3.Distance(enemy.Self.position, target);
=======
        Transform selfTransform = enemy.Self;
        Vector3 target = enemy.Player.position;
        Vector3 currentPosition = selfTransform.position;
        float dist = Vector3.Distance(currentPosition, target);
>>>>>>> main

        if (dist <= enemy.AttackRange)
        {
            if (debug) Debug.Log("[BT] MoveToPlayerAction: Reached target");
            return NodeState.Success;
        }

<<<<<<< HEAD
        float step = enemy.MoveSpeed * Time.deltaTime;
        enemy.Self.position = Vector3.MoveTowards(enemy.Self.position, target, step);
=======
        if (rb != null)
        {
            Vector2 rbPosition = rb.position;
            float horizontalSign = Mathf.Sign(target.x - rbPosition.x);
            Vector2 groundOrigin = PlatformingNavigator.GroundProbeOrigin(selfTransform);

            bool gapAhead = Mathf.Abs(horizontalSign) > 0.01f && PlatformingNavigator.IsGapAhead(groundOrigin, new Vector2(horizontalSign, 0f), gapCheckDistance, groundCheckDepth, groundMask);
            bool obstacleAhead = Mathf.Abs(horizontalSign) > 0.01f && PlatformingNavigator.DetectObstacle(groundOrigin + Vector2.up * 0.45f, new Vector2(horizontalSign, 0f), obstacleCheckDistance, obstacleMask, out _);
            bool needsHeightAdjust = PlatformingNavigator.ShouldJumpForHeight(currentPosition, target, 0.75f);
            bool grounded = PlatformingNavigator.IsGrounded(groundOrigin, groundCheckDepth, groundMask);
            bool hasHeadroom = PlatformingNavigator.HasHeadroom(groundOrigin + Vector2.up * 0.2f, 1.6f, obstacleMask);

            float desiredSpeed = enemy.MoveSpeed;
            float desiredVelX = horizontalSign * desiredSpeed;
            if (gapAhead && !needsHeightAdjust)
            {
                desiredVelX = 0f;
            }

            float newVelX = Mathf.Lerp(rb.linearVelocity.x, desiredVelX, 0.25f);
            rb.linearVelocity = new Vector2(newVelX, rb.linearVelocity.y);

            if ((gapAhead || obstacleAhead || (needsHeightAdjust && hasHeadroom)) && grounded && Time.time >= lastJumpTime + jumpCooldown)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                lastJumpTime = Time.time;
            }

            selfTransform.position = rb.position;

            enemy.Blackboard?.Set("MoveGapAhead", gapAhead);
            enemy.Blackboard?.Set("MoveObstacleAhead", obstacleAhead);
        }
        else
        {
            float step = enemy.MoveSpeed * Time.deltaTime;
            Vector3 moveTarget = target;
            if (PlatformingNavigator.ShouldJumpForHeight(currentPosition, target, 0.75f))
            {
                moveTarget.y = Mathf.Max(target.y, currentPosition.y + 0.75f);
            }

            selfTransform.position = Vector3.MoveTowards(currentPosition, moveTarget, step);
        }
>>>>>>> main

        if (debug) Debug.Log("[BT] MoveToPlayerAction: Moving towards player");
        return NodeState.Running;
    }
}
