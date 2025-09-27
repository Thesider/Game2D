using StateMachine;
using UnityEngine;

// Base state for enemies: uses IEnemy and an animator abstraction so nodes don't depend on Unity Animator.
public class EnemyBaseState : BaseState
{
    protected readonly IEnemy enemy;
    protected readonly IEnemyAnimator animator;

    protected EnemyBaseState(IEnemy enemy, IEnemyAnimator animator)
    {
        this.enemy = enemy;
        this.animator = animator;
    }

    public override void onEnter() { }
    public override void onExit() { }
    public override void onUpdate() { }
    public override void onFixedUpdate() { }
}
