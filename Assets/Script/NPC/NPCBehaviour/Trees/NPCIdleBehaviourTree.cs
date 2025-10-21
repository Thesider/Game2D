public class NPCIdleBehaviourTree : NPCBehaviourTree
{
    public NPCIdleBehaviourTree(INPC npc) : base(npc)
    {
    }

    protected override Node CreateTree()
    {
        var scan = new ScanForThreatsAction(npc);
        var alive = new IsAliveCondition(npc);

        var idleSequence = new Sequence();
        idleSequence.AddChild(alive);

        var rootSequence = new Sequence();
        rootSequence.AddChild(scan);
        rootSequence.AddChild(idleSequence);

        return rootSequence;
    }
}
