using UnityEngine;

public class PlayerCrouchState : PlayerGroundedState {
    public PlayerCrouchState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
        : base(player, stateMachine, playerData, animBoolName) { }

    public override void Enter() {
        base.Enter();
        // Stop horizontal movement while crouching
        player.SetVelocityX(0f);
    }

    public override void LogicUpdate() {
        base.LogicUpdate();
        if (!isExitingState) {
            // Exit crouch when input released
            if (!player.inputHandler.crouchInput) {
                if (player.inputHandler.normInputX != 0) {
                    stateMachine.ChangeState(player.moveState);
                } else {
                    stateMachine.ChangeState(player.idleState);
                }
            }
        }
    }
}
