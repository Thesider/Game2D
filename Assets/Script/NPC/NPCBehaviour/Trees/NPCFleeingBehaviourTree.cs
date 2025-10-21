using UnityEngine;

public class NPCFleeingBehaviourTree : NPCBehaviourTree
{
    private readonly float fleeDistance;

    public NPCFleeingBehaviourTree(INPC npc, float fleeDistance) : base(npc)
    {
        this.fleeDistance = Mathf.Max(0.5f, fleeDistance);
    }

    protected override Node CreateTree()
    {
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

        return rootSequence;
    }
}
