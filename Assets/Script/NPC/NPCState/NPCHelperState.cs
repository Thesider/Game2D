using UnityEngine;

public class NPCHelperState : NPCStateWithBehaviour
{
<<<<<<< HEAD
    public NPCHelperState(INPC npc, bool debug = false) : base(npc, debug)
    {
        SetTickInterval(0.15f);
=======
    private readonly NPCHelperBehaviourTree helperBehaviour;

    public NPCHelperState(INPC npc, bool debug = false) : base(npc, debug)
    {
        SetTickInterval(0.15f);
        helperBehaviour = new NPCHelperBehaviourTree(npc);
>>>>>>> main
    }

    protected override void BuildTree()
    {
        if (root != null) return;
<<<<<<< HEAD

        var scan = new ScanForThreatsAction(npc);
        var isAlive = new IsAliveCondition(npc);
        var enemiesNearby = new AreEnemiesNearbyCondition(npc);
        var playerNearby = new IsPlayerNearbyCondition(npc);
        var combat = new CombatAction(npc);
        var follow = new FollowPlayerAction(npc);

        var combatSequence = new Sequence();
        combatSequence.AddChild(isAlive);
        combatSequence.AddChild(enemiesNearby);
        combatSequence.AddChild(combat);

        var followSequence = new Sequence();
        followSequence.AddChild(isAlive);
        followSequence.AddChild(playerNearby);
        followSequence.AddChild(follow);

        var behaviourSelector = new Selector();
        behaviourSelector.AddChild(combatSequence);
        behaviourSelector.AddChild(followSequence);

        var rootSequence = new Sequence();
        rootSequence.AddChild(scan);
        rootSequence.AddChild(behaviourSelector);

        root = rootSequence;
=======
        root = helperBehaviour.Build();
    }

    public override void onExit()
    {
        base.onExit();
        helperBehaviour.Reset();
>>>>>>> main
    }
}
