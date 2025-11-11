using StateMachine;
using UnityEngine;

public abstract class BossBaseState : BaseState
{
    protected readonly BossController boss;
    protected readonly IAnimator animator;

    protected BossBaseState(BossController boss, IAnimator animator)
    {
        this.boss = boss;
        this.animator = animator ?? boss?.Animator;
    }

    protected BossController Boss => boss;
    protected Transform Player => boss?.Player;
    protected bool HasPlayer => boss != null && boss.HasPlayer;
    protected bool IsAlive => boss != null && boss.IsAlive;
    protected float DistanceToPlayer => boss?.DistanceToPlayer() ?? float.PositiveInfinity;
    protected bool DebugEnabled => boss != null && boss.DebugEnabled;
}
