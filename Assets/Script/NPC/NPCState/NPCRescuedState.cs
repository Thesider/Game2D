using UnityEngine;

public class NPCRescuedState : NPCStateWithBehaviour
{
    private readonly float recoveryDuration;
    private float recoveryTimer;
    private bool isRecovered;
    private const string RecoveredKey = "IsRecovered";

    public NPCRescuedState(INPC npc, float recoveryDuration = 3f, bool debug = false) : base(npc, debug)
    {
        this.recoveryDuration = Mathf.Max(0f, recoveryDuration);
        SetTickInterval(0.25f);
    }

    public override void onEnter()
    {
        base.onEnter();

        recoveryTimer = 0f;
        isRecovered = recoveryDuration <= 0f;
        npc.Blackboard?.Set(RecoveredKey, isRecovered);

        if (Debug.isDebugBuild) Debug.Log($"[Hostage] {npc.NPCId} has been rescued! Recovering...");
        npc.Animator?.SetTrigger("ThankPlayer");
    }

    public override void onUpdate()
    {
        base.onUpdate();

        if (isRecovered) return;

        recoveryTimer += Time.deltaTime;
        if (recoveryTimer >= recoveryDuration)
        {
            isRecovered = true;
            npc.Blackboard?.Set(RecoveredKey, true);
            if (Debug.isDebugBuild) Debug.Log($"[Hostage] {npc.NPCId} has recovered and is ready to move");
        }
    }

    public override void onExit()
    {
        base.onExit();
        npc.Blackboard?.Remove(RecoveredKey);
    }

    protected override void BuildTree()
    {
        if (root != null) return;

        var scan = new ScanForThreatsAction(npc);
        var isAlive = new IsAliveCondition(npc);
        var interactable = new IsInteractableCondition(npc);

        var recoverySequence = new Sequence();
        recoverySequence.AddChild(isAlive);
        recoverySequence.AddChild(interactable);

        var rootSequence = new Sequence();
        rootSequence.AddChild(scan);
        rootSequence.AddChild(recoverySequence);

        root = rootSequence;
    }

    public bool IsRecovered() => isRecovered;
}