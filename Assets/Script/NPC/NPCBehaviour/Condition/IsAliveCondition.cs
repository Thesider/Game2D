public class IsAliveCondition : NPCCondition
{
    public IsAliveCondition(INPC npc) : base(npc)
    {
    }

    protected override bool CheckCondition() => npc.IsAlive;
}
