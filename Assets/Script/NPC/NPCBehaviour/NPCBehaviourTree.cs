public abstract class NPCBehaviourTree
{
    protected readonly INPC npc;
    private Node root;

    protected NPCBehaviourTree(INPC npc)
    {
        this.npc = npc;
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
        root = null;
    }

    protected abstract Node CreateTree();
}
