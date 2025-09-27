using UnityEngine;

// Simple projectile that moves in a direction, damages on contact, then destroys itself.
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    private Vector2 direction;
    private float speed = 8f;
    private float damage = 10f;
    private float lifetime = 4f;
    private GameObject owner;

    public void Initialize(Vector2 direction, float speed, float damage, float lifetime, GameObject owner = null)
    {
        this.direction = direction.normalized;
        this.speed = speed;
        this.damage = damage;
        this.lifetime = lifetime;
        this.owner = owner;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == owner) return;

        // Try to call TakeDamage on the hit object
        other.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == owner) return;

        collision.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

        Destroy(gameObject);
    }
}
