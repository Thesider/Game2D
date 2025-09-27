using UnityEngine;

// Condition: true when enemy health is below or equal to a threshold.
public class LowHealthCondition : Condition
{
    private readonly bool debug;
    private readonly float threshold;

    public LowHealthCondition(IEnemy enemy, float threshold = 30f, bool debug = false) : base(enemy)
    {
        this.threshold = threshold;
        this.debug = debug;
    }

    // Implement the required abstract method from Condition
    protected override bool CheckCondition()
    {
        if (enemy == null)
        {
            if (debug) Debug.Log("[BT] LowHealthCondition: no enemy");
            return false;
        }

        bool result = enemy.Health <= threshold;
        if (debug) Debug.Log($"[BT] LowHealthCondition: {enemy.Health} <= {threshold} => {result}");
        return result;
    }
}