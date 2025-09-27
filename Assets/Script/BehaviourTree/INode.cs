public interface INode
{
    NodeState Evaluate();
}
public enum NodeState
{
    Running,
    Success,
    Failure
}