using UnityEngine;

public enum ManeuverStrategy
{
    Aggressive,   // Close to (slightly inside) attack range
    KeepDistance, // Maintain distance slightly outside attack range
    Flank         // Circle / strafe while staying near attack range
}

public class ManeuverAction : Action
{
    private readonly ManeuverStrategy strategy;
    private readonly bool debug;
    private readonly float desiredDistance;
    private float flankDirection;
    private const float tolerance = 0.25f;

    private readonly float maxDuration;
    private float startTime = -1f;
    private readonly Rigidbody2D rb;
    private readonly LayerMask groundMask;
    private readonly LayerMask obstacleMask;
    private readonly float gapCheckDistance = 0.6f;
    private readonly float groundCheckDepth = 1.4f;
    private readonly float obstacleCheckDistance = 0.45f;
    private readonly float jumpForce = 6f;
    private readonly float jumpCooldown = 0.4f;
    private float lastJumpTime = -5f;

    public ManeuverAction(IEnemy enemy, ManeuverStrategy strategy = ManeuverStrategy.Aggressive, bool debug = false, float maxDuration = 3.0f) : base(enemy)
    {
        this.strategy = strategy;
        this.debug = debug;
        this.maxDuration = maxDuration;

        // Compute desired distance based on the enemy's attack range
        switch (strategy)
        {
            case ManeuverStrategy.Aggressive:
                desiredDistance = enemy.AttackRange * 0.8f;
                break;
            case ManeuverStrategy.KeepDistance:
                desiredDistance = enemy.AttackRange * 1.5f;
                break;
            case ManeuverStrategy.Flank:
            default:
                desiredDistance = enemy.AttackRange * 1.0f;
                break;
        }
        // Randomize initial flank direction
        flankDirection = UnityEngine.Random.value < 0.5f ? -1f : 1f;

        if (enemy is MonoBehaviour mb)
        {
            rb = mb.GetComponent<Rigidbody2D>();
        }

        groundMask = PlatformingNavigator.GroundMask;
        obstacleMask = PlatformingNavigator.ObstacleMask;
    }

    protected override NodeState DoAction()
    {
        if (enemy.Player == null) return NodeState.Failure;

        // init timer on first run
        if (startTime < 0f) startTime = Time.time;

        var selfT = enemy.Self;
        var playerT = enemy.Player;
        Vector3 selfPos = selfT.position;
        Vector3 playerPos = playerT.position;
        Vector3 toPlayer = (playerPos - selfPos);
        float dist = toPlayer.magnitude;

        float step = enemy.MoveSpeed * Time.deltaTime;
        Vector2 desiredVelocity = Vector2.zero;
        Vector3 fallbackTarget = selfPos;
        bool requestJump = false;

        if (maxDuration > 0f && Time.time - startTime > maxDuration)
        {
            if (debug) Debug.Log($"[BT] ManeuverAction: timed out after {maxDuration}s");
            startTime = -1f;
            return NodeState.Success;
        }

        switch (strategy)
        {
            case ManeuverStrategy.Aggressive:
                if (dist <= desiredDistance + tolerance)
                {
                    if (debug) Debug.Log("[BT] ManeuverAction(Aggressive): In position");
                    startTime = -1f;
                    return NodeState.Success;
                }
                // Move toward player
                Vector3 toTarget = (playerPos - selfPos).normalized;
                desiredVelocity = new Vector2(toTarget.x, 0f) * enemy.MoveSpeed;
                fallbackTarget = Vector3.MoveTowards(selfPos, playerPos, step);
                requestJump = PlatformingNavigator.ShouldJumpForHeight(selfPos, playerPos, 0.75f);
                if (debug) Debug.Log("[BT] ManeuverAction(Aggressive): Closing (dist=" + dist + ")");
                break;

            case ManeuverStrategy.KeepDistance:
                if (Mathf.Abs(dist - desiredDistance) <= tolerance)
                {
                    if (debug) Debug.Log("[BT] ManeuverAction(KeepDistance): At desired distance");
                    startTime = -1f;
                    return NodeState.Success;
                }
                if (dist < desiredDistance - tolerance)
                {
                    // Move away from player
                    Vector3 away = (selfPos - playerPos).normalized;
                    desiredVelocity = new Vector2(away.x, 0f) * enemy.MoveSpeed;
                    fallbackTarget = selfPos + away * step;
                    if (debug) Debug.Log("[BT] ManeuverAction(KeepDistance): Backing off (dist=" + dist + ")");
                    break;
                }
                else
                {
                    // Move closer
                    Vector3 toward = (playerPos - selfPos).normalized;
                    desiredVelocity = new Vector2(toward.x, 0f) * enemy.MoveSpeed;
                    fallbackTarget = Vector3.MoveTowards(selfPos, playerPos, step);
                    if (debug) Debug.Log("[BT] ManeuverAction(KeepDistance): Closing slightly (dist=" + dist + ")");
                    break;
                }

            case ManeuverStrategy.Flank:
            default:
                if (dist > enemy.AttackRange + 0.5f)
                {
                    // If too far, close in first
                    Vector3 toward = (playerPos - selfPos).normalized;
                    desiredVelocity = new Vector2(toward.x, 0f) * enemy.MoveSpeed;
                    fallbackTarget = Vector3.MoveTowards(selfPos, playerPos, step);
                    if (debug) Debug.Log("[BT] ManeuverAction(Flank): Closing in (dist=" + dist + ")");
                    break;
                }
                // Strafe around player while maintaining range
                Vector3 forward = toPlayer.normalized;
                Vector3 perp = Vector3.Cross(Vector3.up, forward).normalized * flankDirection;
                Vector3 move = (perp * 0.9f + -forward * 0.1f).normalized; // mostly perpendicular, small inward force
                desiredVelocity = new Vector2(move.x, 0f) * enemy.MoveSpeed;
                fallbackTarget = selfPos + move * step;
                requestJump = PlatformingNavigator.ShouldJumpForHeight(selfPos, playerPos, 0.75f);

                if (Mathf.Abs(desiredVelocity.x) > 0.01f)
                {
                    Vector2 origin = PlatformingNavigator.GroundProbeOrigin(selfT);
                    Vector2 dir = new Vector2(Mathf.Sign(desiredVelocity.x), 0f);
                    bool obstacleAhead = PlatformingNavigator.DetectObstacle(origin + Vector2.up * 0.45f, dir, obstacleCheckDistance, obstacleMask, out _);
                    bool gapAhead = PlatformingNavigator.IsGapAhead(origin, dir, gapCheckDistance, groundCheckDepth, groundMask);
                    if (obstacleAhead || gapAhead)
                    {
                        flankDirection *= -1f;
                        perp = Vector3.Cross(Vector3.up, forward).normalized * flankDirection;
                        move = (perp * 0.9f + -forward * 0.1f).normalized;
                        desiredVelocity = new Vector2(move.x, 0f) * enemy.MoveSpeed;
                        fallbackTarget = selfPos + move * step;
                    }
                }

                if (debug) Debug.Log("[BT] ManeuverAction(Flank): Flanking (dist=" + dist + ")");
                break;
        }

        ApplyMovement(desiredVelocity, fallbackTarget, requestJump, selfPos, step);
        return NodeState.Running;
    }

    private void ApplyMovement(Vector2 desiredVelocity, Vector3 fallbackTarget, bool requestJump, Vector3 currentPosition, float step)
    {
        if (rb != null)
        {
            Vector2 groundOrigin = PlatformingNavigator.GroundProbeOrigin(enemy.Self);
            float horizontalSign = Mathf.Abs(desiredVelocity.x) > 0.01f ? Mathf.Sign(desiredVelocity.x) : 0f;
            bool gapAhead = false;
            bool obstacleAhead = false;
            if (horizontalSign != 0f)
            {
                gapAhead = PlatformingNavigator.IsGapAhead(groundOrigin, new Vector2(horizontalSign, 0f), gapCheckDistance, groundCheckDepth, groundMask);
                obstacleAhead = PlatformingNavigator.DetectObstacle(groundOrigin + Vector2.up * 0.45f, new Vector2(horizontalSign, 0f), obstacleCheckDistance, obstacleMask, out _);
            }

            if (gapAhead)
            {
                desiredVelocity.x *= 0.35f;
            }

            float newVelX = Mathf.Lerp(rb.linearVelocity.x, desiredVelocity.x, 0.2f);
            rb.linearVelocity = new Vector2(newVelX, rb.linearVelocity.y);

            bool grounded = PlatformingNavigator.IsGrounded(groundOrigin, groundCheckDepth, groundMask);
            bool hasHeadroom = PlatformingNavigator.HasHeadroom(groundOrigin + Vector2.up * 0.2f, 1.5f, obstacleMask);

            if (requestJump && grounded && hasHeadroom && Time.time >= lastJumpTime + jumpCooldown)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                lastJumpTime = Time.time;
            }

            enemy.Self.position = rb.position;
        }
        else
        {
            enemy.Self.position = Vector3.MoveTowards(currentPosition, fallbackTarget, step);
        }
    }
}
