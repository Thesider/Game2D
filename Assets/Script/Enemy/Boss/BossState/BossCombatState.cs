using System.Collections;
using UnityEngine;
using StateMachine;

/// <summary>
/// Handles the active combat loop, selecting and executing the boss attack patterns.
/// </summary>
public class BossCombatState : IState
{
    private readonly BossController boss;
    private readonly IAnimator animator;
    private Coroutine combatRoutine;

    public BossCombatState(BossController boss, IAnimator animator)
    {
        this.boss = boss;
        this.animator = animator;
    }

    public void onEnter()
    {
        boss.StopMovement();
        if (boss.DebugEnabled) Debug.Log("[BossCombatState] Enter state.");
        combatRoutine = boss.StartCoroutine(CombatLoop());
    }

    public void onUpdate()
    {
        // Core combat logic is driven by the coroutine loop.
    }

    public void onFixedUpdate()
    {
        // Movement is orchestrated from the coroutine using helper methods on the controller.
    }

    public void onExit()
    {
        if (combatRoutine != null)
        {
            boss.StopCoroutine(combatRoutine);
            combatRoutine = null;
        }
        boss.StopMovement();
        if (boss.DebugEnabled) Debug.Log("[BossCombatState] Exit state.");
    }

    private IEnumerator CombatLoop()
    {
        if (boss.DebugEnabled) Debug.Log("[BossCombatState] Combat loop started.");
        // Small delay gives the animator a frame to settle after the engage animation.
        yield return null;

        while (boss != null && boss.enabled && boss.IsAlive)
        {
            if (!boss.HasPlayer)
            {
                boss.TryFindPlayerReference();
                if (boss.DebugEnabled) Debug.Log("[BossCombatState] No player transform. Trying to find.");
                yield return null;
                continue;
            }

            boss.FacePlayer();

            BossController.BossAttackType attack = boss.SelectRandomAttack();
            if (boss.DebugEnabled) Debug.Log($"[BossCombatState] Selected attack: {attack}");
            switch (attack)
            {
                case BossController.BossAttackType.CloseRange:
                    yield return CloseRangeAttack();
                    break;
                case BossController.BossAttackType.BulletRain:
                    yield return BulletRainAttack();
                    break;
                case BossController.BossAttackType.Charge:
                    yield return ChargeAttack();
                    break;
            }

            yield return boss.RepositionToCombatRangeRoutine();

            float cooldown = Mathf.Max(0f, boss.AttackCooldown);
            if (cooldown > 0f)
            {
                if (boss.DebugEnabled) Debug.Log($"[BossCombatState] Waiting for cooldown {cooldown:F2}s");
                yield return new WaitForSeconds(cooldown);
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator CloseRangeAttack()
    {
        yield return boss.MoveToDistanceRoutine(boss.MeleeRange, boss.MeleeRepositionTolerance, boss.ApproachMoveTimeout);
        if (!boss.HasPlayer)
            yield break;

        if (boss.DebugEnabled) Debug.Log("[BossCombatState] CloseRangeAttack: warning animation.");
        if (!string.IsNullOrEmpty(boss.CloseAttackWarningAnimation))
        {
            animator.Play(boss.CloseAttackWarningAnimation);
            float warn = Mathf.Max(0f, boss.CloseAttackWarningDuration);
            if (warn > 0f)
                yield return new WaitForSeconds(warn);
            else
                yield return null;
        }

        if (!string.IsNullOrEmpty(boss.CloseAttackAnimation))
        {
            if (boss.DebugEnabled) Debug.Log("[BossCombatState] CloseRangeAttack: playing hit animation.");
            animator.Play(boss.CloseAttackAnimation);
        }

        float hitDelay = Mathf.Max(0f, boss.CloseAttackHitDelay);
        if (hitDelay > 0f)
            yield return new WaitForSeconds(hitDelay);
        else
            yield return null;


        if (boss.DebugEnabled)
        {
            Debug.Log("[BossCombatState] CloseRangeAttack: hitbox spawned.");
        }

        yield return boss.ActivateCloseRangeHitbox();

        float recovery = Mathf.Max(0f, boss.CloseAttackRecovery);
        if (recovery > 0f)
            yield return new WaitForSeconds(recovery);
    }

    private IEnumerator BulletRainAttack()
    {
        boss.StopMovement();
        boss.FacePlayer();

        if (boss.DebugEnabled) Debug.Log("[BossCombatState] BulletRainAttack: casting.");
        if (!string.IsNullOrEmpty(boss.BulletRainAnimation))
        {
            animator.Play(boss.BulletRainAnimation);
        }

        float cast = Mathf.Max(0f, boss.BulletRainCastDuration);
        if (cast > 0f)
            yield return new WaitForSeconds(cast);
        else
            yield return null;

        boss.SpawnBulletRain();
        if (boss.DebugEnabled) Debug.Log("[BossCombatState] BulletRainAttack: bullets spawned.");

        float recovery = Mathf.Max(0f, boss.BulletRainRecovery);
        if (recovery > 0f)
            yield return new WaitForSeconds(recovery);
    }

    private IEnumerator ChargeAttack()
    {
        boss.StopMovement();
        boss.FacePlayer();

        if (boss.DebugEnabled) Debug.Log("[BossCombatState] ChargeAttack: windup.");
        if (!string.IsNullOrEmpty(boss.ChargeAnimation))
        {
            animator.Play(boss.ChargeAnimation);
        }

        float windup = Mathf.Max(0f, boss.ChargeWindupDuration);
        if (windup > 0f)
            yield return new WaitForSeconds(windup);

        yield return boss.PerformChargeAttack();
        if (boss.DebugEnabled) Debug.Log("[BossCombatState] ChargeAttack: dash performed.");

        float recovery = Mathf.Max(0f, boss.ChargeRecovery);
        if (recovery > 0f)
            yield return new WaitForSeconds(recovery);
    }
}
