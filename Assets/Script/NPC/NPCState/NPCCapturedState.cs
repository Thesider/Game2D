using UnityEngine;

public class NPCCapturedState : NPCStateWithBehaviour
{
    public NPCCapturedState(INPC npc, bool debug = false) : base(npc, debug)
    {
        SetTickInterval(0.25f);
    }

    protected override void BuildTree()
    {
        if (root != null) return;

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
    }
}