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
        FacePlayer();
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
    private void FacePlayer()
    {
        if (enemy?.Player == null) return;

        Transform self = enemy.Self;
        float deltaX = enemy.Player.position.x - self.position.x;
        if (Mathf.Approximately(deltaX, 0f)) return;

        Vector3 scale = self.localScale;
        float facingSign = Mathf.Sign(deltaX);
        scale.x = Mathf.Abs(scale.x) * facingSign * -1f;
        self.localScale = scale;
    }
}
