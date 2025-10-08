using UnityEngine;

public class NPCFleeingState : NPCStateWithBehaviour
{
    private readonly float fleeDistance;
    private readonly NPCFleeingBehaviourTree fleeingBehaviour;

    public NPCFleeingState(INPC npc, float fleeDistance = 7.5f, bool debug = false) : base(npc, debug)
    {
        this.fleeDistance = Mathf.Max(0.5f, fleeDistance);
        SetTickInterval(0.15f);
        fleeingBehaviour = new NPCFleeingBehaviourTree(npc, this.fleeDistance);
    }

    public override void onEnter()
    {
        base.onEnter();
        npc.Animator?.Play("Fleeing");
        npc.Animator?.SetBool("IsMoving", true);
    }

    public override void onExit()
    {
        base.onExit();
        fleeingBehaviour.Reset();
        npc.Animator?.SetBool("IsMoving", false);
    }

    protected override void BuildTree()
    {
        if (root != null) return;
        root = fleeingBehaviour.Build();
    }
}