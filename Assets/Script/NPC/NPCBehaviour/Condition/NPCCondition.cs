using UnityEngine;

public abstract class NPCCondition : Node
{
    protected readonly INPC npc;

    protected NPCCondition(INPC npc)
    {
        this.npc = npc;
    }

    public override NodeState Evaluate()
    {
        if (npc == null)
        {
            Debug.LogWarning("[NPCCondition] Missing NPC reference");
            return NodeState.Failure;
        }

        return CheckCondition() ? NodeState.Success : NodeState.Failure;
    }

    protected abstract bool CheckCondition();
}
