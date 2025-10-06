using UnityEngine;

public class NPCHostageState : NPCStateWithBehaviour
{
    public NPCHostageState(INPC npc, bool debug = false) : base(npc, debug)
    {
        SetTickInterval(0.2f);
    }

    protected override void BuildTree()
    {
        if (root != null) return;

        var scan = new ScanForThreatsAction(npc);
        var isAlive = new IsAliveCondition(npc);
        var enemiesNearby = new AreEnemiesNearbyCondition(npc);
        var interactable = new IsInteractableCondition(npc);
        var callForHelp = new CallForHelpAction(npc);
        var flee = new FleeToSafetyAction(npc);

        var callSequence = new Sequence();
        callSequence.AddChild(isAlive);
        callSequence.AddChild(enemiesNearby);
        callSequence.AddChild(callForHelp);

        var fleeSequence = new Sequence();
        fleeSequence.AddChild(isAlive);
        fleeSequence.AddChild(enemiesNearby);
        fleeSequence.AddChild(flee);

        var interactSequence = new Sequence();
        interactSequence.AddChild(interactable);

        var behaviourSelector = new Selector();
        behaviourSelector.AddChild(callSequence);
        behaviourSelector.AddChild(fleeSequence);
        behaviourSelector.AddChild(interactSequence);

        var rootSequence = new Sequence();
        rootSequence.AddChild(scan);
        rootSequence.AddChild(behaviourSelector);

        root = rootSequence;
    }
}
