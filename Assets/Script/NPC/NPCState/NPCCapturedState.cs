using UnityEngine;

public class NPCCapturedState : NPCStateWithBehaviour
{
    private readonly NPCCapturedBehaviourTree capturedBehaviour;

    public NPCCapturedState(INPC npc, bool debug = false) : base(npc, debug)
    {
        SetTickInterval(0.25f);
        capturedBehaviour = new NPCCapturedBehaviourTree(npc);
    }

    public override void onEnter()
    {
        base.onEnter();
        npc.Animator?.Play("Captured");
        npc.Animator?.SetBool("IsMoving", false);
    }

    protected override void BuildTree()
    {
        if (root != null) return;
        root = capturedBehaviour.Build();
    }

    public override void onExit()
    {
        base.onExit();
        capturedBehaviour.Reset();
    }
}