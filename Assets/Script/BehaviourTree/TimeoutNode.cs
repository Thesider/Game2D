using UnityEngine;


public class TimeoutNode : Node
{
    private readonly Node child;
    private readonly float timeoutSeconds;
    private readonly System.Action onTimeout;
    private readonly Node onTimeoutNode;
    private float startTime = -1f;

    public TimeoutNode(Node child, float timeoutSeconds, System.Action onTimeout = null)
    {
        this.child = child;
        this.timeoutSeconds = timeoutSeconds;
        this.onTimeout = onTimeout;
    }

    public TimeoutNode(Node child, float timeoutSeconds, Node onTimeoutNode)
    {
        this.child = child;
        this.timeoutSeconds = timeoutSeconds;
        this.onTimeoutNode = onTimeoutNode;
    }

    public override NodeState Evaluate()
    {
        if (startTime < 0f) startTime = Time.time;
        var state = child.Evaluate();
        if (state == NodeState.Success || state == NodeState.Failure)
        {
            startTime = -1f;
            return state;
        }
        // Running
        if (Time.time - startTime > timeoutSeconds)
        {
            if (onTimeout != null) onTimeout.Invoke();
            if (onTimeoutNode != null) onTimeoutNode.Evaluate();
            startTime = -1f;
            if (Debug.isDebugBuild) Debug.Log($"[BT] TimeoutNode: timed out after {timeoutSeconds}s");
            return NodeState.Failure;
        }
        return NodeState.Running;
    }
}
