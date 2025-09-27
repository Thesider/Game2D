using UnityEngine;

// Minimal runtime abstractions used by behaviour-tree nodes.
// Add per-enemy Blackboard and an animation adapter so nodes don't reference Animator directly.
public interface IEnemy
{
    float MoveSpeed { get; }
    float AttackRange { get; }
    float AttackCooldown { get; }
    Transform Player { get; }
    Transform Self { get; }
    float Health { get; set; }

    // Animation abstraction so nodes don't depend on Animator directly.
    IEnemyAnimator Animator { get; }

    // Per-enemy blackboard instance (nodes should use this instead of global/shared blackboards).
    Blackboard Blackboard { get; }

    void TakeDamage(float damage);
}
