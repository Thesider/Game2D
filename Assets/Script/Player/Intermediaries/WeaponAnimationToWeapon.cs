using UnityEngine;

public class WeaponAnimationToWeapon : MonoBehaviour {

    private Weapon weapon;

    private void Start() {
        weapon = GetComponentInParent<Weapon>();
    }

    private void AnimationFinishTrigger() {
        weapon.AnimationFinishTrigger();
    }

}
