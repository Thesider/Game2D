using UnityEngine;


public interface IEnemy
{
    float MoveSpeed { get; }
    float AttackRange { get; }
    float AttackCooldown { get; }
    Transform Player { get; }
    Transform Self { get; }
    float Health { get; set; }

    IAnimator Animator { get; }

    Blackboard Blackboard { get; }

    bool IsAlive { get; }

    void TakeDamage(float damage);
}
