using UnityEngine;

public class NPCRescuedState : NPCStateWithBehaviour
{
    private readonly float recoveryDuration;
    private float recoveryTimer;
    private bool isRecovered;
    private const string RecoveredKey = "IsRecovered";
    private readonly NPCRescuedBehaviourTree rescuedBehaviour;

    public NPCRescuedState(INPC npc, float recoveryDuration = 3f, bool debug = false) : base(npc, debug)
    {
        this.recoveryDuration = Mathf.Max(0f, recoveryDuration);
        SetTickInterval(0.25f);
        rescuedBehaviour = new NPCRescuedBehaviourTree(npc);
    }

    public override void onEnter()
    {
        base.onEnter();

        recoveryTimer = 0f;
        isRecovered = recoveryDuration <= 0f;
        npc.Blackboard?.Set(RecoveredKey, isRecovered);

        npc.Animator?.Play("Rescued");
        npc.Animator?.SetBool("IsMoving", false);

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
        rescuedBehaviour.Reset();
        npc.Blackboard?.Remove(RecoveredKey);
    }

    protected override void BuildTree()
    {
        if (root != null) return;
        root = rescuedBehaviour.Build();
    }

    public bool IsRecovered() => isRecovered;
}