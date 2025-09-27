using UnityEngine;

// Combat state caches its behaviour-tree nodes to avoid allocations each tick.
// It also returns cached nodes to the NodePool on exit (optional single-slot pooling).
public class EnemyCombatState : StateWithBehaviour
{
    // Cached nodes (created once per state instance)
    private Sequence attackSeq;
    private AttackAction attackAction;
    private DebugDecorator attackDebug;
    private MoveToPlayerAction moveAction;
    private DebugDecorator moveDebug;
    private ManeuverAction maneuverAction;
    private DebugDecorator maneuverDebug;
    private TimeoutNode maneuverTimeout;
    private DebugDecorator maneuverTimeoutDebug;
    private Selector rootSel;

    public EnemyCombatState(IEnemy enemy, IEnemyAnimator animator, bool debug = true) : base(enemy, animator, debug)
    {
        // Example: faster decision tick for combat (optional)
        SetTickInterval(0.12f); // ~8 Hz
    }

    public override void onEnter()
    {
        base.onEnter();
        if (animator != null)
            animator.SetBool("IsMoving", true);
    }

    protected override void BuildTree()
    {
        // Build once and reuse cached instances while this state is active.
        if (rootSel != null)
        {
            // already built
            root = rootSel;
            return;
        }

        // ATTACK SEQUENCE
        attackSeq = new Sequence();
        attackSeq.AddChild(new PlayerInRangeCondition(enemy, enemy.AttackRange, debug));
        // Require line of sight before attempting attack
        attackSeq.AddChild(new HasLineOfSightCondition(enemy, debug));

        // AttackAction uses the enemy's animator adapter and blackboard for cooldown timing
        attackAction = new AttackAction(enemy, animator, debug, blackboard);
        attackSeq.AddChild(attackAction);

        attackDebug = new DebugDecorator(attackSeq, "AttackSeq", debug);

        // MOVE-TO-PLAYER (fallback before maneuver) - approach until within attack range
        moveAction = new MoveToPlayerAction(enemy, debug);
        moveDebug = new DebugDecorator(moveAction, "MoveToPlayer", debug);

        // MANEUVER (fallback) - wrapped in a timeout so it can terminate and let Selector re-evaluate Attack
        maneuverAction = new ManeuverAction(enemy, ManeuverStrategy.Flank, debug);
        maneuverDebug = new DebugDecorator(maneuverAction, "Maneuver", debug);
        maneuverTimeout = new TimeoutNode(maneuverDebug, 3.0f); // 3s max maneuver
        maneuverTimeoutDebug = new DebugDecorator(maneuverTimeout, "ManeuverTO", debug);

        // ROOT SELECTOR
        rootSel = new Selector();
        rootSel.AddChild(attackDebug);
        rootSel.AddChild(moveDebug);
        rootSel.AddChild(maneuverTimeoutDebug);

        root = rootSel;
    }

    public override void onUpdate()
    {
        base.onUpdate();
    }

    public override void onExit()
    {
        // Return cached nodes to pool (best-effort) and clear references.
        try
        {
            if (attackAction != null) NodePool.Return(attackAction);
            if (attackSeq != null) NodePool.Return(attackSeq);
            if (attackDebug != null) NodePool.Return(attackDebug);
            if (maneuverAction != null) NodePool.Return(maneuverAction);
            if (maneuverDebug != null) NodePool.Return(maneuverDebug);
            if (rootSel != null) NodePool.Return(rootSel);
        }
        catch { /* pooling is optional — ignore failures */ }

        attackAction = null;
        attackSeq = null;
        attackDebug = null;
        maneuverAction = null;
        maneuverDebug = null;
        rootSel = null;

        base.onExit();
    }

    public override void onFixedUpdate()
    {
    }
}
