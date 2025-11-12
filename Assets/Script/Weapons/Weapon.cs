using UnityEngine;

public class Weapon : MonoBehaviour {
    [SerializeField] protected SO_WeaponData weaponData;


    protected Animator baseAnimator;
    protected Animator weaponAnimator;

    protected PlayerAttackState playerAttackState;


    protected virtual void Start() {
        baseAnimator = transform.Find("Base").GetComponent<Animator>();
        weaponAnimator = transform.Find("Weapon").GetComponent<Animator>();

        gameObject.SetActive(false);
    }

    public virtual void EnterWeapon() {
        gameObject.SetActive(true);

        baseAnimator.SetBool("attack", true);
        weaponAnimator.SetBool("attack", true);
    }

    public virtual void ExitWeapon() {
        baseAnimator.SetBool("attack", false);
        weaponAnimator.SetBool("attack", false);

        gameObject .SetActive(false);
    }

    #region animation trigger
    public virtual void AnimationFinishTrigger() {
        playerAttackState.AnimationFinishTrigger();
    }
    public virtual void AnimationStartMovementTrigger() {
        playerAttackState.SetPlayerVelocity(weaponData.movementSpeed);
    }
    public virtual void AnimationStopMovementTrigger() {
        playerAttackState.SetPlayerVelocity(0f);
    }
    public virtual void AnimationTurnOffFlipTrigger() {
        playerAttackState.SetFLipCheck(false);
    }

    public virtual void AnimationTurnOnFlipTrigger() {
        playerAttackState.SetFLipCheck(true);
    }
    #endregion

    public void InitializeWeapon(PlayerAttackState playerAttackState) {
        this.playerAttackState = playerAttackState;
    } 
}
