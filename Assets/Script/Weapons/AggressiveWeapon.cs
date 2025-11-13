using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System.Linq;

public class AggressiveWeapon : Weapon {
    private List<IDamageable> detectedDamageable = new List<IDamageable>();
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



        foreach (IDamageable item in detectedDamageable.ToList()) {
            item.TakeDamage(detail.damageAmount);
        }
    }

    public void AddToDetected(Collider2D collision) {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if(damageable != null ) {
            detectedDamageable.Add(damageable);
        }
    }
    public void RemoveFromDetected(Collider2D collision) {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null) {
            detectedDamageable.Remove(damageable);
        }


    }

}
