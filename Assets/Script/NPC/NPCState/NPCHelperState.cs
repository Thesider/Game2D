using UnityEngine;

public class NPCHelperState : NPCStateWithBehaviour
{
    private readonly NPCHelperBehaviourTree helperBehaviour;

    public NPCHelperState(INPC npc, bool debug = false) : base(npc, debug)
    {
        SetTickInterval(0.15f);
        helperBehaviour = new NPCHelperBehaviourTree(npc);
    }

    protected override void BuildTree()
    {
        if (root != null) return;
        root = helperBehaviour.Build();
    }

    public override void onExit()
    {
        base.onExit();
        helperBehaviour.Reset();
    }
}
