using UnityEngine;

public class NPCIdleState : NPCStateWithBehaviour
{
    private readonly NPCIdleBehaviourTree idleBehaviour;

    public NPCIdleState(INPC npc, bool debug = false) : base(npc, debug)
    {
        SetTickInterval(0.3f);
        idleBehaviour = new NPCIdleBehaviourTree(npc);
    }

    public override void onEnter()
    {
        base.onEnter();
        npc.Animator?.Play("Idle");
        npc.Animator?.SetBool("IsMoving", false);
    }

    protected override void BuildTree()
    {
        if (root != null) return;
        root = idleBehaviour.Build();
    }

    public override void onExit()
    {
        base.onExit();
        idleBehaviour.Reset();
    }
}