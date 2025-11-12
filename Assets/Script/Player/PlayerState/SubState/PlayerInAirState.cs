using UnityEngine;

public class PlayerInAirState : PlayerState {
    
    private int xInput;

    private bool isGrounded;
    private bool jumpInput;
    private bool jumpInputStop;
    private bool coyoteTime;
    private bool isJumping;

    public PlayerInAirState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void Dochecks() {
        base.Dochecks();
        isGrounded = player.CheckIfGrounded();
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        CheckCoyoteTime();

        xInput = player.inputHandler.normInputX;
        jumpInput = player.inputHandler.jumpInput;
        jumpInputStop = player.inputHandler.jumpInputStop;

        CheckJumpMultiplier();

        if (player.inputHandler.attackInputs[(int)CombatInputs.primary]) {
            stateMachine.ChangeState(player.primaryAttackState);
        } 
        else if (player.inputHandler.attackInputs[(int)CombatInputs.secondary]) {
            stateMachine.ChangeState(player.secondAttackState);
        }
        else if (isGrounded && player.currentVelocity.y < 0.01f) {
            stateMachine.ChangeState(player.landState);
        }
        else if (jumpInput && player.jumpState.CanJump()) {
            player.inputHandler.UseJumpInput();
            stateMachine.ChangeState(player.jumpState);
        }
        else {
            player.CheckFlip(xInput);
            player.SetVelocityX(xInput * playerData.moveSpeed);

            player.anim.SetFloat("yVelocity", player.currentVelocity.y);
            player.anim.SetFloat("xVelocity", Mathf.Abs(player.currentVelocity.x));

        }
    }
    private void CheckJumpMultiplier() {
        if (isJumping) {
            if (jumpInputStop) {
                player.SetVelocityY(player.currentVelocity.y * playerData.variableJumpHeightMultiplier);
                isJumping = false;
            } else if (player.currentVelocity.y <= 0) {
                isJumping = false;
            }
        }

    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }

    private void CheckCoyoteTime() {
        if(coyoteTime && Time.time > startTime + playerData.coyoteTime) {
            coyoteTime = false;
            player.jumpState.DecreaseAmountOfJumpsLeft();
        }
    }
    public void StartCoyoteTime() => coyoteTime = true;
    public void SetIsJumping() => isJumping = true;

}
