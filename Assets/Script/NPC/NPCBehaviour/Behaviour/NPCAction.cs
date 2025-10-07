using UnityEngine;

public abstract class NPCAction : Node
{
    protected readonly INPC npc;

    protected NPCAction(INPC npc)
    {
        this.npc = npc;
    }

    public override NodeState Evaluate()
    {
        if (npc == null)
        {
            Debug.LogWarning("[NPCAction] Missing NPC reference");
            return NodeState.Failure;
        }

        return DoAction();
    }

    protected abstract NodeState DoAction();
}
