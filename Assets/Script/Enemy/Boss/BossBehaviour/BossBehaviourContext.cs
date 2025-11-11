using UnityEngine;

public class BossBehaviourContext
{
    public BossBehaviourContext(BossController boss)
    {
        Boss = boss;
    }

    public BossController Boss { get; }
    public Blackboard Blackboard => Boss?.Blackboard;
    public BossPhaseTracker Phase => Boss?.PhaseTracker;

    public Transform Player => Boss?.Player;
    public bool HasPlayer => Player != null;
    public bool IsAlive => Boss != null && Boss.IsAlive;

    public float DistanceToPlayer { get; private set; } = float.PositiveInfinity;
    public float LastAttackTime { get; set; } = -999f;
    public BossController.BossAttackType LastAttackType { get; set; } = BossController.BossAttackType.CloseRange;

    public void ResetTickState()
    {
        DistanceToPlayer = float.PositiveInfinity;
    }

    public void ResetRuntime()
    {
        LastAttackTime = -999f;
        LastAttackType = BossController.BossAttackType.CloseRange;
        ResetTickState();
    }

    public void UpdateDistanceToPlayer()
    {
        if (!HasPlayer || Boss == null)
        {
            DistanceToPlayer = float.PositiveInfinity;
            return;
        }

        DistanceToPlayer = Vector2.Distance(Boss.transform.position, Player.position);
    }
}
