using UnityEngine;

public class PlayerGroundedState : PlayerState {

    protected int xInput;
    protected int yInput;

    protected bool isTouchingCeiling;

    private bool jumpInput;
    private bool isGrounded;
    private bool dashInput;
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void Dochecks() {
        base.Dochecks();

        isGrounded = player.CheckIfGrounded();
        isTouchingCeiling = player.CheckForCeiling();

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
        yInput = player.inputHandler.normInputY;

        jumpInput = player.inputHandler.jumpInput;
        dashInput = player.inputHandler.dashInput;


        if(player.inputHandler.attackInputs[(int)CombatInputs.primary]) {
            stateMachine.ChangeState(player.primaryAttackState);
        }
        else if(player.inputHandler.attackInputs[(int)CombatInputs.secondary]) {
            stateMachine.ChangeState(player.secondAttackState);
        }
        else if (jumpInput && player.jumpState.CanJump() && !isTouchingCeiling && yInput != -1) {
            player.inputHandler.UseJumpInput();
            stateMachine.ChangeState(player.jumpState);
        }
        else if (!isGrounded) {
            player.inAirState.StartCoyoteTime();
            stateMachine.ChangeState(player.inAirState);
        }
        else if (dashInput && player.dashState.CheckIfDash() && !isTouchingCeiling && yInput != -1) {
            stateMachine.ChangeState(player.dashState);
        }
    }


    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}
