using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System.Linq;

public class AggressiveWeapon : Weapon {
    private List<IDamageable> detectedDamageables = new List<IDamageable>();
    protected List<IKnockBackable> detectedKnockBackable = new List<IKnockBackable>();

    protected SO_AggressiveWeaponData aggressiveWeaponData;

    protected override void Awake() {
        base.Awake();
        if (weaponData.GetType() == typeof(SO_AggressiveWeaponData)) {

            aggressiveWeaponData = (SO_AggressiveWeaponData)weaponData;
        } else {
            Debug.LogError("Wrong weapon Data for the weapon");
        }
    }
    public override void AnimationActionTrigger() {
        base.AnimationActionTrigger();

        CheckMeleeAttack();
    }

    private void CheckMeleeAttack() {
        WeaponAttackDetail detail = aggressiveWeaponData.AttackDetails;

        foreach (IDamageable item in detectedDamageables.ToList()) {
            item.TakeDamage(detail.damageAmount);
        }

        foreach (IKnockBackable item in detectedKnockBackable.ToList()) {
            item.KnockBack(detail.knockBackAngle,detail.knockBackStrength, core.Movement.facingDirection);
        }

    }

    public void AddToDetected(Collider2D collision) {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if(damageable != null ) {
            detectedDamageables.Add(damageable);
        }

        IKnockBackable knockBackable = collision.GetComponent<IKnockBackable>();
        if(knockBackable != null) {
            detectedKnockBackable.Add(knockBackable);
        }
    }
    public void RemoveFromDetected(Collider2D collision) {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null) {
            detectedDamageables.Remove(damageable);
        }

        IKnockBackable knockBackable = collision.GetComponent<IKnockBackable>();
        if (knockBackable != null) {
            detectedKnockBackable.Remove(knockBackable);
        }
    }



}
