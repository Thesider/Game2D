using UnityEngine;

public class NPCCombatState : NPCStateWithBehaviour
{
    public NPCCombatState(INPC npc, bool debug = false) : base(npc, debug)
    {
        SetTickInterval(0.1f);
    }

    protected override void BuildTree()
    {
        if (root != null) return;

        var scan = new ScanForThreatsAction(npc);
        var isAlive = new IsAliveCondition(npc);
        var enemiesNearby = new AreEnemiesNearbyCondition(npc);
        var combat = new CombatAction(npc);

        var combatSequence = new Sequence();
        combatSequence.AddChild(isAlive);
        combatSequence.AddChild(enemiesNearby);
        combatSequence.AddChild(combat);

        var rootSequence = new Sequence();
        rootSequence.AddChild(scan);
        rootSequence.AddChild(combatSequence);

        root = rootSequence;
    }

    public override void onExit()
    {

    }
}