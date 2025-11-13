using UnityEngine;

public abstract class BossStateWithBehaviour : BossBaseState
{
    protected Node root;
    protected readonly bool debug;
    protected readonly BossBehaviourContext context;
    protected float tickInterval = 0.2f;

    private float lastTickTime;

    protected BossStateWithBehaviour(BossController boss, BossBehaviourContext context, IAnimator animator, bool debug = false) : base(boss, animator)
    {
        this.context = context;
        this.debug = debug;
    }

    protected void SetTickInterval(float seconds)
    {
        tickInterval = Mathf.Max(0f, seconds);
    }

    public override void onEnter()
    {
        BuildTree();
        lastTickTime = Time.time;
        if (debug) Debug.Log($"[BossState] Enter {GetType().Name} (tick={tickInterval:F2}s)");
        BTDebug.Add($"[BossState] Enter {GetType().Name}");
    }

    public override void onUpdate()
    {
        if (root == null) return;

        if (tickInterval > 0f && Time.time - lastTickTime < tickInterval) return;
        lastTickTime = Time.time;

        context?.ResetTickState();
        var state = root.Evaluate();
        if (debug) Debug.Log($"[BossState] {GetType().Name} -> {state}");
        BTDebug.Add($"[BossState] {GetType().Name} -> {state}");
    }

    public override void onExit()
    {
        if (debug) Debug.Log($"[BossState] Exit {GetType().Name}");
        BTDebug.Add($"[BossState] Exit {GetType().Name}");
        root = null;
    }

    protected abstract void BuildTree();
}
