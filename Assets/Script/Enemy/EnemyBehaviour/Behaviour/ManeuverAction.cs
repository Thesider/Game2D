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
        flankDirection = UnityEngine.Random.value < 0.5f ? 0.5f : 1f;
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
                selfT.position = Vector3.MoveTowards(selfPos, playerPos, step);
                if (debug) Debug.Log("[BT] ManeuverAction(Aggressive): Closing (dist=" + dist + ")");
                return NodeState.Running;

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
                    selfT.position = selfPos + away * step;
                    if (debug) Debug.Log("[BT] ManeuverAction(KeepDistance): Backing off (dist=" + dist + ")");
                    return NodeState.Running;
                }
                else
                {
                    // Move closer
                    selfT.position = Vector3.MoveTowards(selfPos, playerPos, step);
                    if (debug) Debug.Log("[BT] ManeuverAction(KeepDistance): Closing slightly (dist=" + dist + ")");
                    return NodeState.Running;
                }

            case ManeuverStrategy.Flank:
            default:
                if (dist > enemy.AttackRange + 0.5f)
                {
                    // If too far, close in first
                    selfT.position = Vector3.MoveTowards(selfPos, playerPos, step);
                    if (debug) Debug.Log("[BT] ManeuverAction(Flank): Closing in (dist=" + dist + ")");
                    return NodeState.Running;
                }
                // Strafe around player while maintaining range
                Vector3 forward = toPlayer.normalized;
                Vector3 perp = Vector3.Cross(Vector3.up, forward).normalized * flankDirection;
                Vector3 move = (perp * 0.9f + -forward * 0.1f).normalized; // mostly perpendicular, small inward force
                selfT.position = selfPos + move * step;
                if (debug) Debug.Log("[BT] ManeuverAction(Flank): Flanking (dist=" + dist + ")");
                return NodeState.Running;
        }
    }
}
