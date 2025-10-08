public class IsInteractableCondition : NPCCondition
{
    public IsInteractableCondition(INPC npc) : base(npc)
    {
    }

    protected override bool CheckCondition() => npc.IsInteractable;
}
