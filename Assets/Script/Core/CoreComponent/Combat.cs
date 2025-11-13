using UnityEngine;

public class Combat : CoreComponent, IDamageable {



    public void TakeDamage(float damage) {
        Debug.Log(core.transform.parent.name + "Damaged !!!");
    }
}
