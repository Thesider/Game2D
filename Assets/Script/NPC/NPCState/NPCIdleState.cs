using UnityEngine;

public class NPCIdleState : NPCStateWithBehaviour
{
<<<<<<< HEAD
    public NPCIdleState(INPC npc, bool debug = false) : base(npc, debug)
    {
        SetTickInterval(0.3f);
=======
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
>>>>>>> main
    }

    protected override void BuildTree()
    {
        if (root != null) return;
<<<<<<< HEAD

        var scan = new ScanForThreatsAction(npc);
        var alive = new IsAliveCondition(npc);

        var idleSequence = new Sequence();
        idleSequence.AddChild(alive);

        var rootSequence = new Sequence();
        rootSequence.AddChild(scan);
        rootSequence.AddChild(idleSequence);

        root = rootSequence;
=======
        root = idleBehaviour.Build();
    }

    public override void onExit()
    {
        base.onExit();
        idleBehaviour.Reset();
>>>>>>> main
    }
}