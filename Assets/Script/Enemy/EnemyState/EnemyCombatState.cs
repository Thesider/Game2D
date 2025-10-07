using System;
using UnityEngine;

// Combat state caches its behaviour-tree nodes to avoid allocations each tick.
// It also returns cached nodes to the NodePool on exit (optional single-slot pooling).
public class EnemyCombatState : StateWithBehaviour
{
    // Cached nodes (created once per state instance)
    private PlayerInRangeCondition playerInRangeCondition;
    private HasLineOfSightCondition hasLineOfSightCondition;
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

    public EnemyCombatState(IEnemy enemy, IAnimator animator, bool debug = true) : base(enemy, animator, debug)
    {
        SetTickInterval(0.2f);
    }

    public override void onEnter()
    {
        base.onEnter();

    }

    protected override void BuildTree()
    {
        if (rootSel != null)
        {
            root = rootSel;
            return;
        }

        attackSeq = new Sequence();
        // Cache condition nodes so they can be returned to the NodePool on exit
        playerInRangeCondition = new PlayerInRangeCondition(enemy, enemy.AttackRange, debug);
        attackSeq.AddChild(playerInRangeCondition);
        // Require line of sight before attempting attack
        hasLineOfSightCondition = new HasLineOfSightCondition(enemy, debug);
        attackSeq.AddChild(hasLineOfSightCondition);

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
        try
        {
            if (attackAction != null) NodePool.Return(attackAction);
            if (attackSeq != null) NodePool.Return(attackSeq);
            if (attackDebug != null) NodePool.Return(attackDebug);
            if (playerInRangeCondition != null) NodePool.Return(playerInRangeCondition);
            if (hasLineOfSightCondition != null) NodePool.Return(hasLineOfSightCondition);
            if (moveAction != null) NodePool.Return(moveAction);
            if (moveDebug != null) NodePool.Return(moveDebug);
            if (maneuverAction != null) NodePool.Return(maneuverAction);
            if (maneuverDebug != null) NodePool.Return(maneuverDebug);
            if (maneuverTimeout != null) NodePool.Return(maneuverTimeout);
            if (maneuverTimeoutDebug != null) NodePool.Return(maneuverTimeoutDebug);
            if (rootSel != null) NodePool.Return(rootSel);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during EnemyCombatState cleanup: {ex}");
        }

        attackAction = null;
        attackSeq = null;
        attackDebug = null;
        playerInRangeCondition = null;
        hasLineOfSightCondition = null;
        moveAction = null;
        moveDebug = null;
        maneuverAction = null;
        maneuverDebug = null;
        maneuverTimeout = null;
        maneuverTimeoutDebug = null;
        rootSel = null;
        base.onExit();
    }

    public override void onFixedUpdate()
    {
    }
}
