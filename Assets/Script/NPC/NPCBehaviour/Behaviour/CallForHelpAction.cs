using UnityEngine;

public class CallForHelpAction : NPCAction
{
    private readonly string flagKey;

    public CallForHelpAction(INPC npc, string flagKey = "HelpRequested") : base(npc)
    {
        this.flagKey = flagKey;
    }

    protected override NodeState DoAction()
    {
        npc.Animator?.SetTrigger("CallForHelp");
        npc.Blackboard?.Set(flagKey, true);
        return NodeState.Success;
    }
}
