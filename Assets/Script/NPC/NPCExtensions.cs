using UnityEngine;

public static class NPCExtensions
{
    public static ThreatSummary AssessThreats(this INPC npc, float radius = 6f, LayerMask threatMask = default)
    {
        return NPCThreatUtility.EvaluateThreats(npc, radius, threatMask);
    }

    public static bool HasEnemyNearby(this INPC npc, float radius = 6f, LayerMask threatMask = default)
    {
        return NPCThreatUtility.HasEnemyWithin(npc, radius, threatMask);
    }

    public static bool TryGetNearestEnemy(this INPC npc, float radius, out Transform nearestEnemy, out float distance, LayerMask threatMask = default)
    {
        return NPCThreatUtility.TryGetNearestEnemy(npc, radius, threatMask, out nearestEnemy, out distance);
    }
}
