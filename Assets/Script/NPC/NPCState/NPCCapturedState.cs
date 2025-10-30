using UnityEngine;

public class NPCCapturedState : NPCStateWithBehaviour
{
<<<<<<< HEAD
    public NPCCapturedState(INPC npc, bool debug = false) : base(npc, debug)
    {
        SetTickInterval(0.25f);
=======
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
>>>>>>> main
    }

    protected override void BuildTree()
    {
        if (root != null) return;
<<<<<<< HEAD

        var scan = new ScanForThreatsAction(npc);
        var isAlive = new IsAliveCondition(npc);
        var enemiesNearby = new AreEnemiesNearbyCondition(npc);
        var callForHelp = new CallForHelpAction(npc);

        var callSequence = new Sequence();
        callSequence.AddChild(isAlive);
        callSequence.AddChild(enemiesNearby);
        callSequence.AddChild(callForHelp);

        var rootSequence = new Sequence();
        rootSequence.AddChild(scan);
        rootSequence.AddChild(callSequence);

        root = rootSequence;
=======
        root = capturedBehaviour.Build();
    }

    public override void onExit()
    {
        base.onExit();
        capturedBehaviour.Reset();
>>>>>>> main
    }
}