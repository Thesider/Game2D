using StateMachine;
using UnityEngine;

public class EnemyBaseState : IState
{
    protected readonly EnemyController enemy;
    protected readonly Animator animator;

    protected EnemyBaseState(EnemyController enemy, Animator animator)
    {
        this.enemy = enemy;
        this.animator = animator;
    }
    public virtual void onEnter()
    {
    }
    public virtual void onExit()
    {
    }
    public virtual void onUpdate()
    {
    }
    public virtual void onFixedUpdate()
    {
    }
}

