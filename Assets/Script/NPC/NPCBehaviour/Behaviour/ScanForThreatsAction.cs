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
<<<<<<< HEAD
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
=======
        ThreatSummary summary = NPCThreatUtility.EvaluateThreats(npc, radius, threatMask);
        npc.Blackboard?.Set(enemyCountKey, summary.EnemyCount);
        npc.Blackboard?.Set(enemiesNearbyKey, summary.HasThreat);

        if (!summary.HasThreat)
        {
            npc.Blackboard?.Remove(nearestThreatKey);
            npc.Blackboard?.Remove("NearestThreatTransform");
            npc.Blackboard?.Set("ThreatLevel", 0f);
            npc.Blackboard?.Set("ThreatLevelNormalized", 0f);
            npc.Blackboard?.Set("HighThreat", false);
            npc.Blackboard?.Remove("ThreatCenter");
            return NodeState.Success;
        }

        npc.Blackboard?.Set(nearestThreatKey, summary.NearestPosition);
        npc.Blackboard?.Set("NearestThreatTransform", summary.NearestEnemy);
        npc.Blackboard?.Set("ThreatLevel", summary.ThreatScore);
        npc.Blackboard?.Set("ThreatLevelNormalized", summary.NormalizedThreat);
        npc.Blackboard?.Set("HighThreat", summary.IsHighThreat);
        npc.Blackboard?.Set("ThreatCenter", summary.AveragePosition);
>>>>>>> main
        return NodeState.Success;
    }
}
