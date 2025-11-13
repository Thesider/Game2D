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
        isGrounded = core.CollisionSenes.Ground;

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
        else if (isGrounded && core.Movement.currentVelocity.y < 0.01f) {
            stateMachine.ChangeState(player.landState);
        }
        else if (jumpInput && player.jumpState.CanJump()) {
            player.inputHandler.UseJumpInput();
            stateMachine.ChangeState(player.jumpState);
        }
        else {
            core.Movement.CheckFlip(xInput);
            
            core.Movement.SetVelocityX(xInput * playerData.moveSpeed);

            player.anim.SetFloat("yVelocity", core.Movement.currentVelocity.y);
            player.anim.SetFloat("xVelocity", Mathf.Abs(core.Movement.currentVelocity.x));

        }
    }
    private void CheckJumpMultiplier() {
        if (isJumping) {
            if (jumpInputStop) {
                core.Movement.SetVelocityY(core.Movement.currentVelocity.y * playerData.variableJumpHeightMultiplier);
                isJumping = false;
            } else if (core.Movement.currentVelocity.y <= 0) {
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
