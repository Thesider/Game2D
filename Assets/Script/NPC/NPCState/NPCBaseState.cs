using StateMachine;
using UnityEngine;

public abstract class NPCBaseState : BaseState
{
    protected INPC npc;
    protected Transform npcTransform;
    protected Transform playerTransform;

    public NPCBaseState(INPC npc)
    {
        this.npc = npc;
        this.npcTransform = npc.Transform;
        this.playerTransform = npc.PlayerTransform;
    }

    protected float GetDistanceToPlayer()
    {
        if (playerTransform == null || npcTransform == null) return float.MaxValue;
        return Vector3.Distance(npcTransform.position, playerTransform.position);
    }

    protected Vector3 GetDirectionToPlayer()
    {
        if (playerTransform == null || npcTransform == null) return Vector3.zero;
        return (playerTransform.position - npcTransform.position).normalized;
    }
}