public abstract class EnemyBehaviourTree
{
    protected readonly IEnemy enemy;
    protected readonly IAnimator animator;
    protected readonly Blackboard blackboard;
    protected readonly bool debug;
    private Node root;

    protected EnemyBehaviourTree(IEnemy enemy, IAnimator animator, Blackboard blackboard, bool debug)
    {
        this.enemy = enemy;
        this.animator = animator ?? enemy?.Animator;
        this.blackboard = blackboard ?? enemy?.Blackboard;
        this.debug = debug;
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
