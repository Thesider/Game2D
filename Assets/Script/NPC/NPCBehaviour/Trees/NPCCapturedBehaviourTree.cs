public class NPCCapturedBehaviourTree : NPCBehaviourTree
{
    public NPCCapturedBehaviourTree(INPC npc) : base(npc)
    {
    }

    protected override Node CreateTree()
    {
        var scan = new ScanForThreatsAction(npc);
        var isAlive = new IsAliveCondition(npc);
        var enemiesNearby = new AreEnemiesNearbyCondition(npc);
        var callForHelp = new CallForHelpAction(npc);

        var capturedSequence = new Sequence();
        capturedSequence.AddChild(isAlive);
        capturedSequence.AddChild(enemiesNearby);
        capturedSequence.AddChild(callForHelp);

        var rootSequence = new Sequence();
        rootSequence.AddChild(scan);
        rootSequence.AddChild(capturedSequence);

        return rootSequence;
    }
}
