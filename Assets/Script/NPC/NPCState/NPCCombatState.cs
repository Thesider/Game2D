using UnityEngine;

public class NPCCombatState : NPCStateWithBehaviour
{
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
    }

    protected override void BuildTree()
    {
        if (root != null) return;
        root = combatBehaviour.Build();
    }

    public override void onExit()
    {
        base.onExit();
        combatBehaviour.Reset();
    }
}