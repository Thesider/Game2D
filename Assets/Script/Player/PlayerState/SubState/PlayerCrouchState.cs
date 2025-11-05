using UnityEngine;

public class PlayerCrouchState : PlayerGroundedState {
    public PlayerCrouchState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
        : base(player, stateMachine, playerData, animBoolName) { }

    public override void Enter() {
        base.Enter();
        // Start crouch with no sudden slide
        player.SetVelocityX(0f);
    }

    public override void LogicUpdate() {
        // DO NOT call base.LogicUpdate() to block jump/dash transitions while crouching

        // If lost ground, leave crouch to in-air
        bool grounded = player.CheckIfGrounded();
        if (!grounded) {
            stateMachine.ChangeState(player.inAirState);
            return;
        }

        // Handle exit crouch when button released
        if (!player.inputHandler.crouchInput) {
            if (player.inputHandler.normInputX != 0) {
                stateMachine.ChangeState(player.moveState);
            } else {
                stateMachine.ChangeState(player.idleState);
            }
            return;
        }

        // While crouching: allow slow walk and block jump/dash inputs
        int xInput = player.inputHandler.normInputX;
        if (xInput != 0) {
            player.CheckFlip(xInput);
        }
        player.SetVelocityX(xInput * playerData.crouchMoveSpeed);

        // Clear jump/dash inputs so they don't fire immediately after exiting crouch
        if (player.inputHandler.jumpInput) player.inputHandler.UseJumpInput();
        if (player.inputHandler.dashInput) player.inputHandler.UseDashInput();
    }
}
