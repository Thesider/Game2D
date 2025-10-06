using UnityEngine;

public class NPCFollowingState : NPCStateWithBehaviour
{
    private readonly float followTriggerRange;

    public NPCFollowingState(INPC npc, float followTriggerRange = 12f, bool debug = false) : base(npc, debug)
    {
        this.followTriggerRange = Mathf.Max(0.1f, followTriggerRange);
        SetTickInterval(0.1f);
    }

    protected override void BuildTree()
    {
        if (root != null) return;

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

        root = rootSequence;
    }
}
