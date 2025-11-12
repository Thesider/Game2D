using UnityEngine;

public class PlayerAttackState : PlayerAbilityState {
    private Weapon weapon;

    private int xInput;

    private float  velocityToSet;
    private bool setVelocity;
    private bool shouldCheckFlip;


    public PlayerAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName) {
    }

    public override void Enter() {
        base.Enter();

        setVelocity = false;

        weapon.EnterWeapon();
    }

    public override void Exit() {
        base.Exit();

        weapon.ExitWeapon();
    }

    public override void LogicUpdate() {
        base.LogicUpdate();

        xInput = player.inputHandler.normInputX;

        if(shouldCheckFlip) player.CheckFlip(xInput);

        if (setVelocity) {
            player.SetVelocityX(velocityToSet * player.facingDirection);
            setVelocity = false;
        }
    }

    public void SetWeapon(Weapon weapon) {
        this.weapon = weapon;
        weapon.InitializeWeapon(this);
    }

    public void SetPlayerVelocity(float velocity) {
        player.SetVelocityX(velocity * player.facingDirection);

        velocityToSet = velocity;
        setVelocity = true;
    }

    public void SetFLipCheck(bool value) {
        shouldCheckFlip = value;
    }
    #region Animation Triggers

    public override void AnimationFinishTrigger() {
        base.AnimationFinishTrigger();
        isAbilityDone = true;
    }

    #endregion
}
