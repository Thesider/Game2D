using UnityEngine;

public class PlayerGroundedState : PlayerState {

    protected int xInput;
    private bool jumpInput;
    private bool isGrounded;
    private bool dashInput;
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void Dochecks() {
        base.Dochecks();

        isGrounded = player.CheckIfGrounded();
    }

    public override void Enter() {
        base.Enter();

        player.jumpState.ResetAmountOfJumpsLeft();
        player.dashState.ResetCanDash();
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        xInput = player.inputHandler.normInputX;
        jumpInput = player.inputHandler.jumpInput;
        dashInput = player.inputHandler.dashInput;

        if (jumpInput && player.jumpState.CanJump()) {
            player.inputHandler.UseJumpInput();
            stateMachine.ChangeState(player.jumpState);
        } else if (!isGrounded) {
            player.inAirState.StartCoyoteTime();
            stateMachine.ChangeState(player.inAirState);
        } else if (dashInput && player.dashState.CheckIfDash()) {
            stateMachine.ChangeState(player.dashState);
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}
