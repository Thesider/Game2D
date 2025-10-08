using UnityEngine;

public class EnemyDieState : EnemyBaseState
{
    public EnemyDieState(IEnemy enemy, IAnimator animator) : base(enemy, animator)
    {
    }

    public override void onEnter()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die");
            animator.Play("Die");
        }


    }


    public override void onExit()
    {

    }
}