public class AreEnemiesNearbyCondition : NPCCondition
{
    private readonly string enemiesNearbyKey;

    public AreEnemiesNearbyCondition(INPC npc, string enemiesNearbyKey = "EnemiesNearby") : base(npc)
    {
        this.enemiesNearbyKey = enemiesNearbyKey;
    }

    protected override bool CheckCondition()
    {
        if (npc.Blackboard == null)
        {
            return false;
        }

        return npc.Blackboard.TryGet(enemiesNearbyKey, out bool result) && result;
    }
}
