using StateMachine;
using UnityEngine;

public class EnemyBaseState : BaseState
{
    protected readonly IEnemy enemy;
    protected readonly IAnimator animator;

    protected EnemyBaseState(IEnemy enemy, IAnimator animator)
    {
        this.enemy = enemy;
        this.animator = animator;
    }

    public override void onExit() { }
    public override void onUpdate() { }
    public override void onFixedUpdate() { }
}
