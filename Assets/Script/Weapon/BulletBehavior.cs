using UnityEngine;

public class BulletBehavior : MonoBehaviour {
    [Header("General Bullet Stats")]
    [SerializeField] private LayerMask whatDestroysBullet;
    [SerializeField] private float destroyTime = 2f;


    [Header("Normal Bullet Stats")]
    [SerializeField] private float normalbulletSpeed= 15f;
    [SerializeField] private float normalbulletDamage = 30f;

    [Header("Physics Bullet Stats")]
    [SerializeField] private float physicBulletSpeed = 17.5f;
    [SerializeField] private float physicBulletGravity = 3f;
    [SerializeField] private float physicBulletDamage = 100f;

    private Rigidbody2D rb;
    private float damage;
    public enum BulletType {
        Normal,
        Physic
    }
    public BulletType bulletType;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();

        SetDestroyTime();

        //change RB stats based on bullet type
        SetRBStats();

        //set velocity based on bullet type
        InittializeBulletStats();

    }

    public void FixedUpdate() {
        if (bulletType == BulletType.Physic) transform.right = rb.linearVelocity;
    }


    private void InittializeBulletStats() {
        if(bulletType == BulletType.Normal) {
            SetStraightVelocity();
            damage = normalbulletDamage;
        } else if (bulletType == BulletType.Physic) {
            SetPhysicVelocity();
            damage = physicBulletDamage;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if ((whatDestroysBullet.value & (1 << collision.gameObject.layer)) > 0) {

            //Damage Enemy
            IEnemy iDamageable= collision.GetComponent<IEnemy>();
            if(iDamageable != null) iDamageable.TakeDamage(damage);

            Destroy(gameObject);
        }
    }


    private void SetStraightVelocity() {
        rb.linearVelocity = transform.right * normalbulletSpeed;

    }

    private void SetPhysicVelocity() { 
        rb.linearVelocity = transform.right * physicBulletSpeed;
    }

    public void SetDestroyTime() {
        Destroy(gameObject, destroyTime);
    }

    private void SetRBStats() {
        if (bulletType == BulletType.Normal) rb.gravityScale = 0;
        else if (bulletType == BulletType.Physic) rb.gravityScale = physicBulletGravity;

    }
}
