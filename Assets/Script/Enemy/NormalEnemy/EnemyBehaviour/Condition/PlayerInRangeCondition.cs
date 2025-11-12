using UnityEngine;

public class PlayerInRangeCondition : Condition
{
    private readonly float detectionRange;
    private readonly bool debug;

    public PlayerInRangeCondition(IEnemy enemy, float detectionRange = 6f, bool debug = false) : base(enemy)
    {
        this.detectionRange = detectionRange;
        this.debug = debug;
    }

    protected override bool CheckCondition()
    {
        if (enemy.Player == null) return false;
        float dist = Vector3.Distance(enemy.Self.position, enemy.Player.position);
        bool result = dist <= detectionRange;
        if (debug) Debug.Log($"[BT] PlayerInRangeCondition: {result}");
        return result;
    }
}
