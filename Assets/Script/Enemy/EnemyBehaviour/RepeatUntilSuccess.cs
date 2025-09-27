public class RepeatUntilSuccess : Node
{
    private readonly int maxRepeats;
    private int count;

    public RepeatUntilSuccess(int maxRepeats = int.MaxValue)
    {
        this.maxRepeats = maxRepeats;
    }
    public override NodeState Evaluate()
    {
        if (children == null || children.Count == 0) return NodeState.Failure;

        var child = children[0];

        while (count < maxRepeats)
        {
            var state = child.Evaluate();
            if (state == NodeState.Success)
            {
                count = 0;
                return NodeState.Success;
            }
            if (state == NodeState.Running)
            {
                return NodeState.Running;
            }
            count++;
        }

        count = 0;
        return NodeState.Failure;
    }
}
