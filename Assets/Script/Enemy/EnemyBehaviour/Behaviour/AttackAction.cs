using UnityEngine;
// Try to attack the player when in range and cooldown is ready.
// Triggers the provided animator (if any) when an attack occurs and spawns a bullet prefab if available.
public class AttackAction : Action
{
    private readonly IAnimator animator;
    private readonly bool debug;
    private float lastAttackTime = -999f;
    private readonly Blackboard blackboard;
    private const string LastAttackKey = "lastAttack";

    public AttackAction(IEnemy enemy, IAnimator animator = null, bool debug = false, Blackboard blackboard = null) : base(enemy)
    {
        this.animator = animator;
        this.debug = debug;
        this.blackboard = blackboard ?? enemy.Blackboard;
    }

    protected override NodeState DoAction()
    {
        if (enemy.Player == null) return NodeState.Failure;

        float dist = Vector3.Distance(enemy.Self.position, enemy.Player.position);

        if (dist > enemy.AttackRange)
        {
            if (debug) Debug.Log("[BT] AttackAction: Player out of range");
            return NodeState.Failure;
        }

        // Read last attack time from blackboard if available, otherwise use private field.
        float last = lastAttackTime;
        if (blackboard != null && blackboard.TryGet<float>(LastAttackKey, out var t))
            last = t;

        if (Time.time - last < enemy.AttackCooldown)
        {
            if (debug) Debug.Log("[BT] AttackAction: On cooldown - deferring to maneuver");
            // Keep the node in Running so the Selector will continue evaluating and not treat cooldown as a final Failure.
            return NodeState.Running;
        }

        // Perform attack: update last-attack time
        float now = Time.time;
        if (blackboard != null)
            blackboard.Set(LastAttackKey, now);
        else
            lastAttackTime = now;

        // Trigger animation if available (through IAnimator)
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Spawn projectile if EnemyController provides a prefab
        if (enemy is EnemyController ec && ec.BulletPrefab != null)
        {
            Vector3 spawnPos = ec.AttackPoint != null ? ec.AttackPoint.position : enemy.Self.position;
            Vector3 dir = (enemy.Player.position - spawnPos).normalized;

            var go = GameObject.Instantiate(ec.BulletPrefab, spawnPos, Quaternion.identity);
            var bullet = go.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.Initialize(dir, ec.BulletSpeed, ec.AttackDamage, ec.BulletLifetime, enemy.Self.gameObject);
            }
            else
            {
                if (debug) Debug.LogWarning("[BT] AttackAction: Bullet prefab missing Bullet component.");
            }

            if (debug) Debug.Log("[BT] AttackAction: Spawned bullet");
        }

        if (debug) Debug.Log("[BT] AttackAction: Attack triggered");
        return NodeState.Success;
    }
}
