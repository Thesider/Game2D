using UnityEngine;

public class Combat : CoreComponent, IDamageable, IKnockBackable {
    private bool isKnockbackActive;
    private float knockbackStartTime;
    public void LogicUpdate() {
        CheckKnockback();
    }
    public void TakeDamage(float damage) {
         Debug.Log(core.transform.parent.name + "Damaged !!!");
    }
    public void KnockBack(Vector2 angle, float strength, int direction) {
        core.Movement.SetVelocity(strength, angle, direction);
        core.Movement.canSetVelocity = false;
        isKnockbackActive = true;
        knockbackStartTime = Time.time;
    }
    private void CheckKnockback() {
        if (isKnockbackActive && core.Movement.currentVelocity.y <= 0.01f && core.CollisionSenes.Ground) {
            isKnockbackActive = false;
            core.Movement.canSetVelocity = true;
        }
    }

}
