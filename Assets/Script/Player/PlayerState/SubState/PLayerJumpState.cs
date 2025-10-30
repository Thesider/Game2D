using UnityEngine;

public class PLayerJumpState : PlayerAbilityState {

    private int amountOfJumpsLeft;
    public PLayerJumpState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
        amountOfJumpsLeft = playerData.amountOfJumps;
    }



    public override void Enter() {
        base.Enter();

        player.SetVelocityY(playerData.jumpVelocity);
        isAbilityDone = true;
        amountOfJumpsLeft--;
        player.inAirState.SetIsJumping();

    }


    public bool CanJump() {
        if(amountOfJumpsLeft > 0) {
         return true;
        }
        return false;
    }
    public void ResetAmountOfJumpsLeft() => amountOfJumpsLeft = playerData.amountOfJumps;
    public void DecreaseAmountOfJumpsLeft() => amountOfJumpsLeft--;
    
}
