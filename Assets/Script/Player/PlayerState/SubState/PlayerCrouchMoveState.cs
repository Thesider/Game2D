using UnityEngine;

public class PlayerCrouchMoveState : PlayerGroundedState {
    public PlayerCrouchMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }
    public override void Enter() {
        base.Enter();
        player.SetVelocityZero();
        player.SetColliderHeight(playerData.crouchColliderHeight);
    }
    public override void Exit() {
        base.Exit();
        player.SetColliderHeight(playerData.standColliderHeight);
    }
    public override void LogicUpdate() {
        base.LogicUpdate();

        if (!isExitingState) {
            player.SetVelocityX(playerData.crouchSpeed * player.facingDirection);
            player.CheckFlip(xInput);
            if (xInput == 0) {
                stateMachine.ChangeState(player.crouchIdleState);
            } else if (yInput != -1 && !isTouchingCeiling) {
                stateMachine.ChangeState(player.moveState);
            }
        }
    }
}
