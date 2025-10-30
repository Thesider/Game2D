using UnityEngine;

public class NPCCombatState : NPCStateWithBehaviour
{
<<<<<<< HEAD
    public NPCCombatState(INPC npc, bool debug = false) : base(npc, debug)
    {
        SetTickInterval(0.1f);
=======
    private readonly NPCCombatBehaviourTree combatBehaviour;

    public NPCCombatState(INPC npc, bool debug = false) : base(npc, debug)
    {
        SetTickInterval(0.1f);
        combatBehaviour = new NPCCombatBehaviourTree(npc);
    }

    public override void onEnter()
    {
        base.onEnter();
        npc.Animator?.Play("Combat");
>>>>>>> main
    }

    protected override void BuildTree()
    {
        if (root != null) return;
<<<<<<< HEAD

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
=======
        root = combatBehaviour.Build();
>>>>>>> main
    }

    public override void onExit()
    {
<<<<<<< HEAD

=======
        base.onExit();
        combatBehaviour.Reset();
>>>>>>> main
    }
}