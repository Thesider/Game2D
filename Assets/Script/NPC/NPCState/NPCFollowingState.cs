using UnityEngine;

public class NPCFollowingState : NPCStateWithBehaviour
{
    private readonly float followTriggerRange;
<<<<<<< HEAD
=======
    private readonly NPCFollowingBehaviourTree followingBehaviour;
>>>>>>> main

    public NPCFollowingState(INPC npc, float followTriggerRange = 12f, bool debug = false) : base(npc, debug)
    {
        this.followTriggerRange = Mathf.Max(0.1f, followTriggerRange);
        SetTickInterval(0.1f);
<<<<<<< HEAD
=======
        followingBehaviour = new NPCFollowingBehaviourTree(npc, this.followTriggerRange);
    }

    public override void onEnter()
    {
        base.onEnter();
        npc.Animator?.Play("Following");
        npc.Animator?.SetBool("IsMoving", true);
    }

    public override void onExit()
    {
        base.onExit();
        followingBehaviour.Reset();
        npc.Animator?.SetBool("IsMoving", false);
>>>>>>> main
    }

    protected override void BuildTree()
    {
        if (root != null) return;
<<<<<<< HEAD

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
=======
        root = followingBehaviour.Build();
>>>>>>> main
    }
}
