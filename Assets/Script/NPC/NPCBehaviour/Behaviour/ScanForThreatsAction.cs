using UnityEngine;

public class ScanForThreatsAction : NPCAction
{
    private readonly float radius;
    private readonly LayerMask threatMask;
    private readonly string enemiesNearbyKey;
    private readonly string enemyCountKey;
    private readonly string nearestThreatKey;

    public ScanForThreatsAction(
        INPC npc,
        float radius = 6f,
        LayerMask threatMask = default,
        string enemiesNearbyKey = "EnemiesNearby",
        string enemyCountKey = "EnemyCount",
        string nearestThreatKey = "NearestThreatPosition") : base(npc)
    {
        this.radius = Mathf.Max(0.5f, radius);
        this.threatMask = threatMask == default ? LayerMask.GetMask("Enemy") : threatMask;
        this.enemiesNearbyKey = enemiesNearbyKey;
        this.enemyCountKey = enemyCountKey;
        this.nearestThreatKey = nearestThreatKey;
    }

    protected override NodeState DoAction()
    {
        Vector3 npcPosition = npc.Transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(npcPosition, radius, threatMask);
        npc.Blackboard?.Set(enemyCountKey, hits.Length);
        npc.Blackboard?.Set(enemiesNearbyKey, hits.Length > 0);

        if (hits.Length == 0)
        {
            npc.Blackboard?.Remove(nearestThreatKey);
            return NodeState.Success;
        }

        float closestDistance = float.MaxValue;
        Vector3 closestPosition = npcPosition;

        foreach (Collider2D hit in hits)
        {
            float distance = Vector3.Distance(npcPosition, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPosition = hit.transform.position;
            }
        }

        npc.Blackboard?.Set(nearestThreatKey, closestPosition);
        return NodeState.Success;
    }
}
