using UnityEngine;

public class NPCIdleState : NPCStateWithBehaviour
{
    public NPCIdleState(INPC npc, bool debug = false) : base(npc, debug)
    {
        SetTickInterval(0.3f);
    }

    protected override void BuildTree()
    {
        if (root != null) return;

        var scan = new ScanForThreatsAction(npc);
        var alive = new IsAliveCondition(npc);

        var idleSequence = new Sequence();
        idleSequence.AddChild(alive);

        var rootSequence = new Sequence();
        rootSequence.AddChild(scan);
        rootSequence.AddChild(idleSequence);

        root = rootSequence;
    }
}