using System.Collections.Generic;

public abstract class Node : INode
{
    protected List<Node> children = new List<Node>();

    public void AddChild(Node child) => children.Add(child);

    public abstract NodeState Evaluate();
}