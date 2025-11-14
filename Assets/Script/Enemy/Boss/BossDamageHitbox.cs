using System.Collections;
using UnityEngine;

/// <summary>
/// Runtime-created circular trigger that applies boss damage to the player during melee/charge windows.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(CircleCollider2D))]
public class BossDamageHitbox : MonoBehaviour
{
    private BossController owner;
    private CircleCollider2D trigger;
    private float damage;
    private Vector2 localOffset;
    private bool singleHit;
    private bool damageApplied;
    private Coroutine lifetimeRoutine;

    private void Awake()
    {
        trigger = GetComponent<CircleCollider2D>();
        trigger.isTrigger = true;
        trigger.enabled = false;
    }

    public void Activate(BossController controller, float rawDamage, float radius, Vector2 offset, float lifetime, bool singleHitWindow)
    {
        if (controller == null)
            return;

        owner = controller;
        damage = Mathf.Max(0f, rawDamage);
        localOffset = offset;
        singleHit = singleHitWindow;
        damageApplied = false;

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        transform.localScale = Vector3.one;
        trigger.radius = Mathf.Max(0f, radius);
        trigger.enabled = false;

        UpdateWorldPosition();

        ResolveImmediateContacts();

        if (singleHit && damageApplied)
        {
            Deactivate();
            return;
        }

        trigger.enabled = true;

        if (lifetimeRoutine != null)
        {
            StopCoroutine(lifetimeRoutine);
            lifetimeRoutine = null;
        }

        if (lifetime > 0f)
        {
            lifetimeRoutine = StartCoroutine(LifetimeRoutine(lifetime));
        }
    }

    public void Deactivate()
    {
        if (lifetimeRoutine != null)
        {
            StopCoroutine(lifetimeRoutine);
            lifetimeRoutine = null;
        }

        trigger.enabled = false;
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }

        owner = null;
    }

    private IEnumerator LifetimeRoutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Deactivate();
    }

    private void LateUpdate()
    {
        if (!trigger.enabled || owner == null)
            return;

        UpdateWorldPosition();
    }

    private void UpdateWorldPosition()
    {
        if (owner == null)
            return;

        Transform bossTransform = owner.transform;
        float facing = bossTransform.localScale.x < 0f ? -1f : 1f;
        Vector3 offset = new Vector3(localOffset.x * facing, localOffset.y, 0f);
        transform.position = bossTransform.position + offset;
    }

    private void ResolveImmediateContacts()
    {
        float worldRadius = trigger.radius * Mathf.Abs(transform.lossyScale.x);
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, worldRadius);
        for (int i = 0; i < hits.Length; i++)
        {
            if (TryDealDamage(hits[i]))
            {
                damageApplied = true;
                if (singleHit)
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!trigger.enabled || owner == null)
            return;

        if (singleHit && damageApplied)
            return;

        if (TryDealDamage(other))
        {
            damageApplied = true;
            if (singleHit)
            {
                Deactivate();
            }
        }
    }

    private bool TryDealDamage(Collider2D collider)
    {
        if (collider == null || owner == null)
            return false;

        Transform target = collider.transform;
        if (owner != null && owner.Player != null)
        {
            if (target == owner.Player || target.IsChildOf(owner.Player))
            {
                return owner.ApplyDamageToPlayer(damage);
            }
        }

        var status = target.GetComponentInParent<PlayerStatus>();
        if (status != null)
        {
            status.TakeDamage(Mathf.RoundToInt(damage));
            return true;
        }

        return false;
    }
}
