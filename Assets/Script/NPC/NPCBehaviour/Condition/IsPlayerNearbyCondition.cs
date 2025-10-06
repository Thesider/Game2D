using UnityEngine;

public class IsPlayerNearbyCondition : NPCCondition
{
    private readonly float range;

    public IsPlayerNearbyCondition(INPC npc, float range = 3f) : base(npc)
    {
        this.range = Mathf.Max(0.1f, range);
    }

    protected override bool CheckCondition()
    {
        if (npc.PlayerTransform == null)
        {
            return false;
        }

        return Vector3.Distance(npc.Transform.position, npc.PlayerTransform.position) <= range;
    }
}
