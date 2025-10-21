public class EnemyCombatBehaviourTree : EnemyBehaviourTree
{
    private readonly ManeuverStrategy maneuverStrategy;
    private readonly float maneuverTimeoutSeconds;

    private PlayerInRangeCondition playerInRangeCondition;
    private HasLineOfSightCondition hasLineOfSightCondition;
    private Sequence attackSequence;
    private AttackAction attackAction;
    private DebugDecorator attackDebug;
    private MoveToPlayerAction moveAction;
    private DebugDecorator moveDebug;
    private ManeuverAction maneuverAction;
    private DebugDecorator maneuverDebug;
    private TimeoutNode maneuverTimeout;
    private DebugDecorator maneuverTimeoutDebug;
    private Selector rootSelector;

    public EnemyCombatBehaviourTree(
        IEnemy enemy,
        IAnimator animator,
        Blackboard blackboard,
        bool debug,
        ManeuverStrategy maneuverStrategy = ManeuverStrategy.Flank,
        float maneuverTimeoutSeconds = 3.0f) : base(enemy, animator, blackboard, debug)
    {
        this.maneuverStrategy = maneuverStrategy;
        this.maneuverTimeoutSeconds = maneuverTimeoutSeconds;
    }

    protected override Node CreateTree()
    {
        attackSequence = new Sequence();

        playerInRangeCondition = new PlayerInRangeCondition(enemy, enemy.AttackRange, debug);
        attackSequence.AddChild(playerInRangeCondition);

        hasLineOfSightCondition = new HasLineOfSightCondition(enemy, debug);
        attackSequence.AddChild(hasLineOfSightCondition);

        attackAction = new AttackAction(enemy, animator, debug, blackboard);
        attackSequence.AddChild(attackAction);

        attackDebug = new DebugDecorator(attackSequence, "AttackSeq", debug);

        moveAction = new MoveToPlayerAction(enemy, debug);
        moveDebug = new DebugDecorator(moveAction, "MoveToPlayer", debug);

        maneuverAction = new ManeuverAction(enemy, maneuverStrategy, debug);
        maneuverDebug = new DebugDecorator(maneuverAction, "Maneuver", debug);

        maneuverTimeout = new TimeoutNode(maneuverDebug, maneuverTimeoutSeconds);
        maneuverTimeoutDebug = new DebugDecorator(maneuverTimeout, "ManeuverTO", debug);

        rootSelector = new Selector();
        rootSelector.AddChild(attackDebug);
        rootSelector.AddChild(moveDebug);
        rootSelector.AddChild(maneuverTimeoutDebug);

        return rootSelector;
    }

    protected override void OnReset()
    {
        ReturnNode(ref attackAction);
        ReturnNode(ref attackSequence);
        ReturnNode(ref attackDebug);
        ReturnNode(ref playerInRangeCondition);
        ReturnNode(ref hasLineOfSightCondition);
        ReturnNode(ref moveAction);
        ReturnNode(ref moveDebug);
        ReturnNode(ref maneuverAction);
        ReturnNode(ref maneuverDebug);
        ReturnNode(ref maneuverTimeout);
        ReturnNode(ref maneuverTimeoutDebug);
        ReturnNode(ref rootSelector);
    }

    private void ReturnNode<T>(ref T node) where T : class
    {
        if (node == null) return;
        NodePool.Return(node);
        node = null;
    }
}
