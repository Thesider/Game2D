using UnityEngine;

// Returns true if player in attack range AND cooldown is ready.
public class AttackReadyCondition : Condition
{
    private readonly bool debug;
    public AttackReadyCondition(IEnemy enemy, bool debug = false) : base(enemy)
    {
        this.debug = debug;
    }

    protected override bool CheckCondition()
    {
        if (enemy.Player == null) return false;
        float dist = Vector3.Distance(enemy.Self.position, enemy.Player.position);
        bool inRange = dist <= enemy.AttackRange;

        if (enemy is MonoBehaviour mb)
        {
            mb.GetComponentInChildren<Light>().enabled = inRange;

        }
        if (debug) Debug.Log($"[BT] AttackReadyCondition: inRange={inRange}");
        return inRange;
    }
}
