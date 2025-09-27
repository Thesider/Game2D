using UnityEngine;

public class DebugDecorator : Node
{
    private readonly Node child;
    private readonly string name;
    private readonly bool debug;

    public DebugDecorator(Node child, string name, bool debug = true)
    {
        this.child = child;
        this.name = name;
        this.debug = debug;
    }

    public override NodeState Evaluate()
    {
        var state = child.Evaluate();
        string msg = $"[BT Debug] {name} -> {state}";
        if (debug) Debug.Log(msg);
        BTDebug.Add(msg);
        return state;
    }
}
