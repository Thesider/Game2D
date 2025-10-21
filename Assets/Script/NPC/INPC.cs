using UnityEngine;

public interface INPC
{
    string NPCId { get; }
    NPCType Type { get; }
    Transform Transform { get; }
    Transform PlayerTransform { get; }
    bool IsInteractable { get; }
    bool IsAlive { get; }

    Blackboard Blackboard { get; }

    IAnimator Animator { get; }
    void OnInteract(Transform player);
    void TakeDamage(float damage);
    void Die();
    void UpdateNPC();

    UnityEngine.Rigidbody2D Rigidbody { get; }
    float MoveSpeed { get; }
    bool IsGrounded { get; }

    void Move(UnityEngine.Vector2 velocity);

    void Jump(float force);
}


public enum NPCType
{
    Shop,
    Hostage,
    Helper,
    Quest,
    Civilian
}