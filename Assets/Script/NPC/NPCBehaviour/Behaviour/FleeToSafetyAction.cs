using UnityEngine;

public class FleeToSafetyAction : NPCAction
{
    private readonly float fleeDistance;
    private readonly float moveSpeed;
    private readonly string threatKey;

    public FleeToSafetyAction(INPC npc, float fleeDistance = 6f, float moveSpeed = 4f, string threatKey = "NearestThreatPosition") : base(npc)
    {
        this.fleeDistance = Mathf.Max(0.5f, fleeDistance);
        this.moveSpeed = Mathf.Max(0.1f, moveSpeed);
        this.threatKey = threatKey;
    }

    protected override NodeState DoAction()
    {
        Vector3 threatPosition = npc.Transform.position;

        if (npc.Blackboard != null && npc.Blackboard.TryGet(threatKey, out Vector3 storedThreat))
        {
            threatPosition = storedThreat;
        }
        else if (npc.PlayerTransform != null)
        {
            threatPosition = npc.PlayerTransform.position;
        }

        Vector3 toThreat = npc.Transform.position - threatPosition;
        Vector3 direction;

        if (toThreat == Vector3.zero)
        {
            Vector2 random = Random.insideUnitCircle.normalized;
            direction = new Vector3(random.x, random.y, 0f);
        }
        else
        {
            direction = toThreat.normalized;
        }
        Vector3 target = npc.Transform.position + direction * fleeDistance;

        npc.Transform.position = Vector3.MoveTowards(npc.Transform.position, target, moveSpeed * Time.deltaTime);
        npc.Animator?.SetBool("IsMoving", true);

        float currentDistance = Vector3.Distance(npc.Transform.position, threatPosition);
        if (currentDistance >= fleeDistance)
        {
            npc.Animator?.SetBool("IsMoving", false);
            return NodeState.Success;
        }

        return NodeState.Running;
    }
}
