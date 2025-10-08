public class NPCRescuedBehaviourTree : NPCBehaviourTree
{
    public NPCRescuedBehaviourTree(INPC npc) : base(npc)
    {
    }

    protected override Node CreateTree()
    {
        var scan = new ScanForThreatsAction(npc);
        var isAlive = new IsAliveCondition(npc);
        var interactable = new IsInteractableCondition(npc);

        var recoverySequence = new Sequence();
        recoverySequence.AddChild(isAlive);
        recoverySequence.AddChild(interactable);

        var rootSequence = new Sequence();
        rootSequence.AddChild(scan);
        rootSequence.AddChild(recoverySequence);

        return rootSequence;
    }
}
