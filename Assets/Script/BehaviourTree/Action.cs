public abstract class Action : Node
{
    protected IEnemy enemy;

    public Action(IEnemy enemy) => this.enemy = enemy;

    public override NodeState Evaluate() => DoAction();

    protected abstract NodeState DoAction();
}
