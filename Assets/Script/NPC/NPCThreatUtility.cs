using UnityEngine;

public struct ThreatSummary
{
    public int EnemyCount;
    public Transform NearestEnemy;
    public float NearestDistance;
    public Vector3 NearestPosition;
    public float ThreatScore;
    public Vector3 AveragePosition;

    public bool HasThreat => EnemyCount > 0;
    public float NormalizedThreat => EnemyCount == 0 ? 0f : Mathf.Clamp01(ThreatScore / (EnemyCount * 4f));
    public bool IsHighThreat => ThreatScore >= Mathf.Max(2f, EnemyCount * 1.5f);
}

public static class NPCThreatUtility
{
    private static readonly Collider2D[] ThreatBuffer = new Collider2D[32];

    public static ThreatSummary EvaluateThreats(INPC npc, float radius, LayerMask threatMask)
    {
        ThreatSummary summary = new ThreatSummary();

        if (npc == null || npc.Transform == null)
        {
            return summary;
        }

        if (radius <= 0f)
        {
            radius = 1f;
        }

        if (threatMask == 0)
        {
            threatMask = LayerMask.GetMask("Enemy");
            if (threatMask == 0)
            {
                threatMask = Physics2D.DefaultRaycastLayers;
            }
        }

        Vector3 origin = npc.Transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius, threatMask);
        summary.EnemyCount = hits != null ? hits.Length : 0;

        if (summary.EnemyCount == 0)
        {
            summary.NearestDistance = float.MaxValue;
            return summary;
        }

        float threatAccumulator = 0f;
        float nearestDistance = float.MaxValue;
        Vector3 nearestPos = origin;
        Transform nearestTransform = null;
        Vector3 centroid = Vector3.zero;

        foreach (var col in hits)
        {
            if (col == null) continue;

            Transform threatTransform = col.transform;
            IEnemy enemy = col.GetComponent<IEnemy>() ?? col.GetComponentInParent<IEnemy>();

            Vector3 threatPosition = threatTransform.position;
            centroid += threatPosition;

            float distance = Vector3.Distance(origin, threatPosition);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPos = threatPosition;
                nearestTransform = threatTransform;
            }

            float threatScore = ComputeThreatScore(enemy, distance, radius);
            threatAccumulator += threatScore;
        }

        summary.NearestDistance = nearestDistance;
        summary.NearestPosition = nearestPos;
        summary.NearestEnemy = nearestTransform;
        summary.AveragePosition = centroid / Mathf.Max(1, summary.EnemyCount);
        summary.ThreatScore = threatAccumulator;

        return summary;
    }

    public static bool TryGetNearestEnemy(INPC npc, float radius, LayerMask threatMask, out Transform nearestEnemy, out float distance)
    {
        ThreatSummary summary = EvaluateThreats(npc, radius, threatMask);
        nearestEnemy = summary.NearestEnemy;
        distance = summary.NearestDistance;
        return summary.HasThreat;
    }

    public static bool HasEnemyWithin(INPC npc, float radius, LayerMask threatMask)
    {
        ThreatSummary summary = EvaluateThreats(npc, radius, threatMask);
        return summary.HasThreat;
    }

    public static float DetermineThreatScore(INPC npc, float radius, LayerMask threatMask)
    {
        ThreatSummary summary = EvaluateThreats(npc, radius, threatMask);
        return summary.ThreatScore;
    }

    private static float ComputeThreatScore(IEnemy enemy, float distance, float radius)
    {
        if (enemy == null)
        {
            float distanceWeight = Mathf.Max(0f, 1f - (distance / Mathf.Max(radius, 0.1f)));
            return 0.5f + distanceWeight;
        }

        float normalizedDistance = Mathf.Clamp01(distance / Mathf.Max(radius, 0.1f));
        float distanceFactor = 1f - normalizedDistance; // closer -> larger threat

        float health = Mathf.Max(0f, enemy.Health);
        float healthFactor = Mathf.Clamp(health / 100f, 0.2f, 2.5f);

        float attackRangeFactor = Mathf.Clamp(enemy.AttackRange / Mathf.Max(radius, 0.1f), 0.2f, 2f);
        float attackCooldownFactor = Mathf.Clamp01(1f / Mathf.Max(0.1f, enemy.AttackCooldown));

        float baseThreat = 0.75f + attackRangeFactor * 0.5f + attackCooldownFactor * 0.75f;
        return baseThreat * distanceFactor * healthFactor;
    }
}
