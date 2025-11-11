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
            // Disable colliders so the dead enemy no longer interacts with physics/triggers
            try
            {
                var self = enemy?.Self;
                if (self != null)
                {
                    var go = self.gameObject;
                    // Disable 2D colliders
                    foreach (var c2 in go.GetComponentsInChildren<Collider2D>(true))
                        c2.enabled = false;
                }
            }
            catch { }
        }


    }


    public override void onExit()
    {

    }
}