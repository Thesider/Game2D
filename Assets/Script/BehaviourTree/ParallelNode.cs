
public class ParallelNode : Node
{
    private readonly int successRequired;
    private readonly int failureRequired;

    public ParallelNode(int successRequired = -1, int failureRequired = -1)
    {
        // -1 means "all"
        this.successRequired = successRequired;
        this.failureRequired = failureRequired;
    }


    public override NodeState Evaluate()
    {
        if (children.Count == 0) return NodeState.Failure;

        int succ = 0;
        int fail = 0;
        bool anyRunning = false;

        foreach (var c in children)
        {
            var st = c.Evaluate();
            switch (st)
            {
                case NodeState.Success:
                    succ++;
                    break;
                case NodeState.Failure:
                    fail++;
                    break;
                case NodeState.Running:
                    anyRunning = true;
                    break;
            }
        }

        int requiredSucc = successRequired < 0 ? children.Count : successRequired;
        int requiredFail = failureRequired < 0 ? children.Count : failureRequired;

        if (succ >= requiredSucc) return NodeState.Success;
        if (fail >= requiredFail) return NodeState.Failure;
        if (anyRunning) return NodeState.Running;

        if (succ > fail) return NodeState.Success;
        if (fail > succ) return NodeState.Failure;

        return NodeState.Running;
    }
}