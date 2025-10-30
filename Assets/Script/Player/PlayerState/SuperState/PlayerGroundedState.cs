using UnityEngine;

public class PlayerGroundedState : PlayerState {

    protected int xInput;
    private bool jumpInput;
    private bool isGrounded;
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void Dochecks() {
        base.Dochecks();

        isGrounded = player.CheckIfGrounded();
    }

    public override void Enter() {
        base.Enter();

        player.jumpState.ResetAmountOfJumpsLeft();
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        xInput = player.inputHandler.normInputX;

        jumpInput = player.inputHandler.jumpInput;

        if (jumpInput && player.jumpState.CanJump()) {
            player.inputHandler.UseJumpInput();
            stateMachine.ChangeState(player.jumpState);
        } else if (!isGrounded) {
            player.inAirState.StartCoyoteTime();
            stateMachine.ChangeState(player.inAirState);
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}
