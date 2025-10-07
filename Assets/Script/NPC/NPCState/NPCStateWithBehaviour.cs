using UnityEngine;
using StateMachine;

public abstract class NPCStateWithBehaviour : BaseState
{
    protected readonly INPC npc;
    protected readonly bool debug;
    protected Node root;
    protected float tickInterval = 0.2f;

    private float lastTick;

    protected NPCStateWithBehaviour(INPC npc, bool debug = false)
    {
        this.npc = npc;
        this.debug = debug;
    }

    protected void SetTickInterval(float seconds)
    {
        tickInterval = Mathf.Max(0f, seconds);
    }

    public override void onEnter()
    {
        BuildTree();
        lastTick = Time.time;
        if (debug) Debug.Log($"[NPCState] Enter {GetType().Name}");
    }

    public override void onUpdate()
    {
        if (root == null) return;

        if (tickInterval > 0f && Time.time - lastTick < tickInterval) return;
        lastTick = Time.time;

        var state = root.Evaluate();
        if (debug) Debug.Log($"[NPCState] {GetType().Name} -> {state}");
    }

    public override void onExit()
    {
        root = null;
        if (debug) Debug.Log($"[NPCState] Exit {GetType().Name}");
    }

    protected abstract void BuildTree();
}