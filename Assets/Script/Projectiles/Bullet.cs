using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

// Simple projectile that moves in a direction, damages on contact, then returns to a pool instead of destroying itself.
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    private Vector2 direction = Vector2.right;
    private float speed = 8f;
    private float damage = 10f;
    private float lifetime = 4f;
    private GameObject owner;

    private ObjectPool<Bullet> pool;
    private Coroutine lifetimeRoutine;
    private bool released;

    internal void SetPool(ObjectPool<Bullet> sourcePool)
    {
        pool = sourcePool;
    }

    public void Initialize(Vector2 direction, float speed, float damage, float lifetime, GameObject owner = null)
    {
        this.direction = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.right;
        this.speed = speed;
        this.damage = damage;
        this.lifetime = lifetime;
        this.owner = owner;
        released = false;

        if (lifetimeRoutine != null)
        {
            StopCoroutine(lifetimeRoutine);
            lifetimeRoutine = null;
        }

        if (gameObject.activeInHierarchy && lifetime > 0f)
        {
            lifetimeRoutine = StartCoroutine(LifetimeCountdown(lifetime));
        }
    }

    private IEnumerator LifetimeCountdown(float duration)
    {
        yield return new WaitForSeconds(duration);
        ReleaseToPool();
    }

    private void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == owner) return;

        other.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        ReleaseToPool();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == owner) return;

        collision.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        ReleaseToPool();
    }

    internal void ReleaseToPool()
    {
        if (released)
            return;

        released = true;

        if (lifetimeRoutine != null)
        {
            StopCoroutine(lifetimeRoutine);
            lifetimeRoutine = null;
        }

        pool?.Release(this);
    }

    internal void ResetAfterRelease()
    {
        if (lifetimeRoutine != null)
        {
            StopCoroutine(lifetimeRoutine);
            lifetimeRoutine = null;
        }

        owner = null;
        direction = Vector2.right;
        speed = 0f;
        damage = 0f;
        lifetime = 0f;
        released = false;
    }
}
