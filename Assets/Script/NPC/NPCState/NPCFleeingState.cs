using UnityEngine;

public class NPCFleeingState : NPCStateWithBehaviour
{
    private readonly float fleeDistance;

    public NPCFleeingState(INPC npc, float fleeDistance = 7.5f, bool debug = false) : base(npc, debug)
    {
        this.fleeDistance = Mathf.Max(0.5f, fleeDistance);
        SetTickInterval(0.15f);
    }

    protected override void BuildTree()
    {
        if (root != null) return;

        var scan = new ScanForThreatsAction(npc);
        var isAlive = new IsAliveCondition(npc);
        var enemiesNearby = new AreEnemiesNearbyCondition(npc);
        var flee = new FleeToSafetyAction(npc, fleeDistance);

        var fleeSequence = new Sequence();
        fleeSequence.AddChild(isAlive);
        fleeSequence.AddChild(enemiesNearby);
        fleeSequence.AddChild(flee);

        var rootSequence = new Sequence();
        rootSequence.AddChild(scan);
        rootSequence.AddChild(fleeSequence);

        root = rootSequence;
    }
}