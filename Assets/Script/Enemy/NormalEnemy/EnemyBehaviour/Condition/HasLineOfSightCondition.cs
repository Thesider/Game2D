using UnityEngine;


public class HasLineOfSightCondition : Condition
{
    private readonly float maxDistance;
    private readonly int layerMask;
    private readonly bool debug;

    public HasLineOfSightCondition(IEnemy enemy, bool debug = false, float maxDistance = Mathf.Infinity, int layerMask = ~0)
        : base(enemy)
    {
        this.maxDistance = maxDistance;
        this.layerMask = layerMask;
        this.debug = debug;
    }

    protected override bool CheckCondition()
    {
        if (enemy.Player == null || enemy.Self == null) return false;

        Vector3 from = enemy.Self.position + Vector3.up * 0.5f;
        Vector3 to = enemy.Player.position + Vector3.up * 0.5f;
        Vector3 dir = to - from;
        float dist = dir.magnitude;
        if (dist > maxDistance) return false;

        if (Physics.Raycast(from, dir.normalized, out var hit, dist, layerMask))
        {
            if (debug) Debug.Log($"[BT] HasLineOfSight: blocked by {hit.collider.gameObject.name}");
            return false;
        }

        if (debug) Debug.Log("[BT] HasLineOfSight: clear");
        return true;
    }
}