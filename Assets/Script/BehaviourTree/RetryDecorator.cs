using UnityEngine;

// Decorator that retries its child up to maxRetries times.
// Returns Success if child returns Success within retry limit.
// If maxRetries == 0 => no retry (behaves like a passthrough).
public class RetryDecorator : Node
{
    private readonly Node child;
    private readonly int maxRetries;
    private int attempts = 0;
    private readonly bool debug;
    private readonly string name;

    public RetryDecorator(Node child, int maxRetries = 1, bool debug = false, string name = "Retry")
    {
        this.child = child;
        this.maxRetries = Mathf.Max(0, maxRetries);
        this.debug = debug;
        this.name = name;
    }

    public override NodeState Evaluate()
    {
        if (maxRetries == 0)
            return child.Evaluate();

        var state = child.Evaluate();

        if (state == NodeState.Success)
        {
            attempts = 0;
            if (debug) Debug.Log($"[BT] {name}: child succeeded");
            return NodeState.Success;
        }

        if (state == NodeState.Failure)
        {
            attempts++;
            if (debug) Debug.Log($"[BT] {name}: child failed (attempt {attempts}/{maxRetries})");
            if (attempts <= maxRetries)
            {
                // allow retry next tick
                return NodeState.Running;
            }
            attempts = 0;
            if (debug) Debug.Log($"[BT] {name}: exhausted retries -> Failure");
            return NodeState.Failure;
        }

        // Running: let it continue
        return NodeState.Running;
    }
}
