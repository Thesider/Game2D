using System;
using UnityEngine;

public class PlayerDashState : PlayerAbilityState {
    public bool canDash { get; private set; }
    //private bool isHolding;
    private Vector2 dashDirection;
    private Vector2 rawDashInput;

    private float prevLinearDamping;
    private float prevGravityScale;
    private float lastDashTime;
    public PlayerDashState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    
    
    }

    public override void Enter() {
        base.Enter();
        canDash = false;
        player.inputHandler.UseDashInput();

        //isHolding = true;
        dashDirection = Vector2.right * player.facingDirection;
        // Freeze gravity and record current damping to avoid drifting during dash
        prevGravityScale = player.rb.gravityScale;
        prevLinearDamping = player.rb.linearDamping;
        player.rb.gravityScale = 0f;
        player.rb.linearDamping = 0f;
        
    }

    public override void Exit() {
        base.Exit();
        // Restore physics properties after dashing
        player.rb.linearDamping = prevLinearDamping;
        player.rb.gravityScale = prevGravityScale;
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        //if (isHolding) {
        //    // Read dash direction from input; fallback to facing direction if no input
        //    rawDashInput = player.inputHandler.rawMovementInput;
        //    if (rawDashInput.sqrMagnitude > 0.01f) {
        //        dashDirection = rawDashInput.normalized;
        //    } else {
        //        dashDirection = Vector2.right * player.facingDirection;
        //    }

        //    // Start the actual dash immediately (no aim-hold window implemented)
        //    isHolding = false;




        //}
        rawDashInput = player.inputHandler.rawMovementInput;
        if (rawDashInput.sqrMagnitude > 0.01f) {
            dashDirection = rawDashInput.normalized;
        } else {
            dashDirection = Vector2.right * player.facingDirection;
        }
        // Apply dash velocity during dash time window
        player.rb.linearVelocity = dashDirection * playerData.dashVelocity;

        if (Time.time >= startTime + playerData.dashTime) {
            // End dash
            lastDashTime = Time.time;
            // Stop movement instantly to avoid post-dash drifting
            player.rb.linearVelocity = Vector2.zero;
            isAbilityDone = true;
        }
    }


    public Boolean CheckIfDash() {
        return canDash && Time.time >= lastDashTime + playerData.dashCooldown;
    }
    public void ResetCanDash() => canDash = true;


}
