using UnityEngine;

public class NPCFollowingBehaviourTree : NPCBehaviourTree
{
    private readonly float followTriggerRange;

    public NPCFollowingBehaviourTree(INPC npc, float followTriggerRange) : base(npc)
    {
        this.followTriggerRange = Mathf.Max(0.1f, followTriggerRange);
    }

    protected override Node CreateTree()
    {
        var scan = new ScanForThreatsAction(npc);
        var isAlive = new IsAliveCondition(npc);
        var playerNearby = new IsPlayerNearbyCondition(npc, followTriggerRange);
        var follow = new FollowPlayerAction(npc);

        var followSequence = new Sequence();
        followSequence.AddChild(isAlive);
        followSequence.AddChild(playerNearby);
        followSequence.AddChild(follow);

        var rootSequence = new Sequence();
        rootSequence.AddChild(scan);
        rootSequence.AddChild(followSequence);

        return rootSequence;
    }
}
