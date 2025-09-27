public class EnemyDieState : EnemyBaseState
{
    public EnemyDieState(IEnemy enemy, IEnemyAnimator animator) : base(enemy, animator)
    {
    }

    public override void onEnter()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
    }

    public override void onExit()
    {
    }
}