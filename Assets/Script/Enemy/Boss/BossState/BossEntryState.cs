using System.Collections;
using UnityEngine;
using StateMachine;

/// <summary>
/// Waits for the player to enter the detection radius, then triggers the engage sequence.
/// </summary>
public class BossEntryState : IState
{
    private readonly BossController boss;
    private readonly IAnimator animator;
    private Coroutine engageRoutine;
    private float lastDebugReport;

    public BossEntryState(BossController boss, IAnimator animator)
    {
        this.boss = boss;
        this.animator = animator;
    }

    public void onEnter()
    {
        boss.StopMovement();
        boss.ResetCombatEntryRequest();
        lastDebugReport = Time.time;
        if (boss.DebugEnabled) Debug.Log("[BossEntryState] Entered. Waiting for player detection.");
    }

    public void onUpdate()
    {
        if (boss == null || !boss.enabled)
            return;

        if (boss.DebugEnabled && Time.time - lastDebugReport >= 0.5f)
        {
            lastDebugReport = Time.time;
            if (!boss.HasPlayer)
            {
                Debug.Log("[BossEntryState] No player reference set. Awaiting detection.");
            }
            else
            {
                float distance = boss.DistanceToPlayer();
                bool withinRange = boss.IsPlayerWithinDetectionRange();
                Debug.Log($"[BossEntryState] Player distance: {distance:F2}. Within detection: {withinRange}.");
            }
        }

        if (!boss.HasPlayer)
        {
            boss.TryFindPlayerReference();
            if (boss.DebugEnabled) Debug.Log("[BossEntryState] Trying to locate player reference.");
            return;
        }

        if (engageRoutine == null && !boss.HasPendingCombatEntry && boss.IsPlayerWithinDetectionRange())
        {
            if (boss.DebugEnabled) Debug.Log("[BossEntryState] Player detected within range. Starting engage sequence.");
            engageRoutine = boss.StartCoroutine(EngageSequence());
        }
    }

    public void onFixedUpdate()
    {
    }

    public void onExit()
    {
        if (engageRoutine != null)
        {
            boss.StopCoroutine(engageRoutine);
            engageRoutine = null;
        }
        if (boss.DebugEnabled) Debug.Log("[BossEntryState] Exiting state.");
    }

    private IEnumerator EngageSequence()
    {
        boss.StopMovement();
        boss.FacePlayer();
        if (boss.DebugEnabled) Debug.Log("[BossEntryState] Engage sequence started.");

        if (!string.IsNullOrEmpty(boss.EngageAnimation))
        {
            animator.Play(boss.EngageAnimation);
        }

        float duration = Mathf.Max(0f, boss.EngageAnimationDuration);
        if (duration > 0f)
        {
            yield return new WaitForSeconds(duration);
        }
        else
        {
            yield return null;
        }

        boss.QueueCombatEntry();
        if (boss.DebugEnabled) Debug.Log("[BossEntryState] Engage animation complete. Queued combat entry.");
        engageRoutine = null;
    }
}
