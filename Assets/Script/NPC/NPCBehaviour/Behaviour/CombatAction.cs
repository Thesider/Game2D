using UnityEngine;

public class CombatAction : NPCAction
{
    private readonly float attackCooldown;
    private readonly string cooldownKey;

    public CombatAction(INPC npc, float attackCooldown = 1.0f, string cooldownKey = "NpcLastAttackTime") : base(npc)
    {
        this.attackCooldown = Mathf.Max(0.1f, attackCooldown);
        this.cooldownKey = cooldownKey;
    }

    protected override NodeState DoAction()
    {
        if (npc.Blackboard == null)
        {
            return NodeState.Failure;
        }

        float lastAttack = 0f;
        npc.Blackboard.TryGet(cooldownKey, out lastAttack);

        if (Time.time - lastAttack < attackCooldown)
        {
            return NodeState.Running;
        }

        npc.Animator?.SetTrigger("Attack");
        npc.Blackboard.Set(cooldownKey, Time.time);

        return NodeState.Success;
    }
}
