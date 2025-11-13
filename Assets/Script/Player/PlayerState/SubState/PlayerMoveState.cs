using UnityEngine;

public class PlayerMoveState : PlayerGroundedState {
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void Dochecks() {
        base.Dochecks();
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        core.Movement.CheckFlip(xInput);

        core.Movement.SetVelocityX(xInput * playerData.moveSpeed);


        if (!isExitingState) {
            if (xInput == 0) {
                stateMachine.ChangeState(player.idleState);
            } else if (yInput == -1 ) {
                stateMachine.ChangeState(player.crouchMoveState);
            }
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}
