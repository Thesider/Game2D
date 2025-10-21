public class NPCHelperBehaviourTree : NPCBehaviourTree
{
    public NPCHelperBehaviourTree(INPC npc) : base(npc)
    {
    }

    protected override Node CreateTree()
    {
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

        return rootSequence;
    }
}
