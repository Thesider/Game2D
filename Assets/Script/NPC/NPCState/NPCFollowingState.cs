using UnityEngine;

public class NPCFollowingState : NPCStateWithBehaviour
{
    private readonly float followTriggerRange;
    private readonly NPCFollowingBehaviourTree followingBehaviour;

    public NPCFollowingState(INPC npc, float followTriggerRange = 12f, bool debug = false) : base(npc, debug)
    {
        this.followTriggerRange = Mathf.Max(0.1f, followTriggerRange);
        SetTickInterval(0.1f);
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
    }

    protected override void BuildTree()
    {
        if (root != null) return;
        root = followingBehaviour.Build();
    }
}
