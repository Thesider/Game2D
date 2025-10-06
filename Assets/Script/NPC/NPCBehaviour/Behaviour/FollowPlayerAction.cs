using UnityEngine;

public class FollowPlayerAction : NPCAction
{
    private readonly float stoppingDistance;
    private readonly string moveSpeedKey;

    public FollowPlayerAction(INPC npc, float stoppingDistance = 1.5f, string moveSpeedKey = "MoveSpeed") : base(npc)
    {
        this.stoppingDistance = Mathf.Max(0.1f, stoppingDistance);
        this.moveSpeedKey = moveSpeedKey;
    }

    protected override NodeState DoAction()
    {
        if (npc.PlayerTransform == null)
        {
            return NodeState.Failure;
        }

        Vector3 current = npc.Transform.position;
        Vector3 target = npc.PlayerTransform.position;
        float distance = Vector3.Distance(current, target);

        if (distance <= stoppingDistance)
        {
            npc.Animator?.SetBool("IsMoving", false);
            return NodeState.Success;
        }

        float speed = npc.Blackboard != null && npc.Blackboard.TryGet(moveSpeedKey, out float storedSpeed)
            ? storedSpeed
            : 3f;

        Vector3 next = Vector3.MoveTowards(current, target, speed * Time.deltaTime);
        npc.Transform.position = next;
        npc.Animator?.SetBool("IsMoving", true);

        return NodeState.Running;
    }
}
