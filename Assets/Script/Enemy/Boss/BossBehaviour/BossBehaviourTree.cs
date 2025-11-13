using UnityEngine;

public abstract class BossBehaviourTree
{
    protected readonly BossBehaviourContext context;
    protected readonly BossController boss;
    protected readonly Blackboard blackboard;
    protected readonly bool debug;

    private Node root;

    protected BossBehaviourTree(BossBehaviourContext context, bool debug)
    {
        this.context = context;
        boss = context?.Boss;
        this.debug = debug || boss?.DebugBehaviour == true;
        blackboard = context?.Blackboard;
    }

    public Node Build()
    {
        if (root == null)
        {
            root = CreateTree();
        }
        return root;
    }

    public void Reset()
    {
        OnReset();
        root = null;
    }

    protected abstract Node CreateTree();

    protected virtual void OnReset()
    {
    }
}
