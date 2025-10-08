using UnityEngine;

// Move towards the player until within attack range.
public class MoveToPlayerAction : Action
{
    private readonly bool debug;

    public MoveToPlayerAction(IEnemy enemy, bool debug = false) : base(enemy)
    {
        this.debug = debug;
    }

    protected override NodeState DoAction()
    {
        if (enemy.Player == null) return NodeState.Failure;

        Vector3 target = enemy.Player.position;
        float dist = Vector3.Distance(enemy.Self.position, target);

        if (dist <= enemy.AttackRange)
        {
            if (debug) Debug.Log("[BT] MoveToPlayerAction: Reached target");
            return NodeState.Success;
        }

        float step = enemy.MoveSpeed * Time.deltaTime;
        enemy.Self.position = Vector3.MoveTowards(enemy.Self.position, target, step);

        if (debug) Debug.Log("[BT] MoveToPlayerAction: Moving towards player");
        return NodeState.Running;
    }
}
