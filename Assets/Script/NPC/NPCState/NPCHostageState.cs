using UnityEngine;

public class NPCHostageState : NPCStateWithBehaviour
{
    private readonly NPCHostageBehaviourTree hostageBehaviour;

    public NPCHostageState(INPC npc, bool debug = false) : base(npc, debug)
    {
        SetTickInterval(0.2f);
        hostageBehaviour = new NPCHostageBehaviourTree(npc);
    }

    protected override void BuildTree()
    {
        if (root != null) return;
        root = hostageBehaviour.Build();
    }

    public override void onExit()
    {
        base.onExit();
        hostageBehaviour.Reset();
    }
}
