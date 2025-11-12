using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    private float idleTimer;
    private readonly float idleDuration = 1.5f;

    public EnemyIdleState(IEnemy enemy, IAnimator animator) : base(enemy, animator)
    {
    }

    public override void onEnter()
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }

        idleTimer = 0f;
    }

    public override void onUpdate()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration)
        {
            idleTimer = 0f;
        }
    }

    public override void onExit()
    {

    }

    public override void onFixedUpdate()
    {
    }
}