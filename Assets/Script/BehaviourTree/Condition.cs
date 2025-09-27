using UnityEngine;

public abstract class Condition : Node
{
    protected IEnemy enemy;

    public Condition(IEnemy enemy) => this.enemy = enemy;

    public override NodeState Evaluate() => CheckCondition() ? NodeState.Success : NodeState.Failure;

    protected abstract bool CheckCondition();
}