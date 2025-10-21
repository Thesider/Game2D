using UnityEngine;

public class EnemyCombatState : StateWithBehaviour
{
    private readonly EnemyCombatBehaviourTree combatBehaviour;

    public EnemyCombatState(IEnemy enemy, IAnimator animator, bool debug = true) : base(enemy, animator, debug)
    {
        SetTickInterval(0.2f);
        combatBehaviour = new EnemyCombatBehaviourTree(enemy, animator, blackboard, debug);
    }

    public override void onEnter()
    {
        base.onEnter();
        animator?.Play("Running");
        animator?.SetBool("IsMoving", true);
    }

    protected override void BuildTree()
    {
        if (root != null) return;
        root = combatBehaviour.Build();
    }

    public override void onUpdate()
    {
        base.onUpdate();
    }

    public override void onExit()
    {
        combatBehaviour.Reset();
        animator?.SetBool("IsMoving", false);
        base.onExit();
    }

    public override void onFixedUpdate()
    {
    }
}
