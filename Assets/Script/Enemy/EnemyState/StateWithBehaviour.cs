using UnityEngine;

// State base that owns a behaviour tree and uses the per-enemy blackboard.
// Improvements:
// - Throttled decision ticks via tickInterval (set 0 for per-frame).
// - Uses IEnemy.Blackboard (per-enemy) instead of creating a new one here.
// - Does NOT clear the enemy blackboard on state exit (avoid cross-state data loss).
// - Helpers to tune tick interval per-state.
public abstract class StateWithBehaviour : EnemyBaseState
{
    // Per-state view of the enemy's blackboard (do not Clear() this here).
    protected Blackboard blackboard => enemy.Blackboard;
    protected Node root;
    protected readonly bool debug;

    // Decision tick interval in seconds. Default 0.2 => 5 Hz.
    // Set to 0 for per-frame evaluation.
    protected float tickInterval = 0.2f;
    private float lastTickTime = 0f;

    protected StateWithBehaviour(IEnemy enemy, IEnemyAnimator animator, bool debug = false) : base(enemy, animator)
    {
        this.debug = debug;
        // Use the enemy's blackboard; do not create a new one per-state.
    }

    // Allow derived states to change tick interval.
    protected void SetTickInterval(float seconds)
    {
        tickInterval = Mathf.Max(0f, seconds);
    }

    public override void onEnter()
    {
        BuildTree();
        lastTickTime = Time.time;
        if (debug) Debug.Log($"[State] Enter {GetType().Name} - tree built: {root != null} (tickInterval={tickInterval}s)");
        BTDebug.Add($"[State] Enter {GetType().Name}");
    }

    public override void onUpdate()
    {
        if (root == null) return;

        // Throttle decision ticks unless tickInterval == 0
        if (tickInterval > 0f)
        {
            if (Time.time - lastTickTime < tickInterval) return;
            lastTickTime = Time.time;
        }

        var state = root.Evaluate();
        if (debug) Debug.Log($"[State] {GetType().Name} - root returned {state}");
        BTDebug.Add($"[State] {GetType().Name} -> {state}");
    }

    public override void onExit()
    {
        BTDebug.Add($"[State] Exit {GetType().Name}");
        root = null;
        if (debug) Debug.Log($"[State] Exit {GetType().Name}");
    }

    protected abstract void BuildTree();
}
