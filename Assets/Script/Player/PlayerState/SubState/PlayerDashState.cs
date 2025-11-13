using System;
using System.Collections;
using UnityEngine;

public class PlayerDashState : PlayerAbilityState {
    public bool canDash { get; private set; }

    private Vector2 dashDirection;
    private Vector2 rawDashInput;

    private float prevLinearDamping;
    private float prevGravityScale;
    private float lastDashTime;

    private Coroutine dashTimerCoroutine;
    private bool dashActive;

    public PlayerDashState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
        : base(player, stateMachine, playerData, animBoolName) { }

    public override void Enter() {
        base.Enter();
        canDash = false;
        player.inputHandler.UseDashInput();

        dashDirection = Vector2.right * core.Movement.facingDirection;

        // Freeze physics to avoid sliding drift
        prevGravityScale = player.rb.gravityScale;
        prevLinearDamping = player.rb.linearDamping;
        player.rb.gravityScale = 0f;
        player.rb.linearDamping = 0f;

        dashActive = true;

        if (dashTimerCoroutine != null) {
            player.StopCoroutine(dashTimerCoroutine);
        }
        dashTimerCoroutine = player.StartCoroutine(DashTimer());
    }

    public override void Exit() {
        base.Exit();

        // Ensure velocity stopped
        
        core.Movement.SetVelocityZero();

        // Restore physics
        player.rb.gravityScale = prevGravityScale;
        player.rb.linearDamping = prevLinearDamping;

        if (dashTimerCoroutine != null) {
            player.StopCoroutine(dashTimerCoroutine);
            dashTimerCoroutine = null;
        }

        dashActive = false;
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        if (!dashActive) return; // do not continue applying dash velocity after finished

        rawDashInput = player.inputHandler.rawMovementInput;
        if (rawDashInput.sqrMagnitude > 0.01f) {
            dashDirection = rawDashInput.normalized;
        } else {
            dashDirection = Vector2.right * core.Movement.facingDirection;
        }

        player.rb.linearVelocity = dashDirection * playerData.dashVelocity;
    }

    private IEnumerator DashTimer() {
        yield return new WaitForSeconds(playerData.dashTime);

        // Finish dash
        lastDashTime = Time.time;
        dashActive = false;

        // Stop movement instantly
        core.Movement.SetVelocityZero();

        isAbilityDone = true;

        dashTimerCoroutine = null;
    }

    public bool CheckIfDash() {
        return canDash && Time.time >= lastDashTime + playerData.dashCooldown;
    }

    public void ResetCanDash() => canDash = true;
}