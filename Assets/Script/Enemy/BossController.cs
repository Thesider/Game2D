using System.Collections;
using UnityEngine;
using StateMachine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour, IEnemy
{
    public enum BossAttackType
    {
        CloseRange,
        BulletRain,
        Charge
    }

    [Header("Core References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private bool debugBehaviour;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 250f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 7f;
    [SerializeField] private float combatRange = 4f;
    [SerializeField] private float meleeRange = 1.75f;
    [SerializeField] private float attackCooldown = 1.25f;

    [Header("Animation States")]
    [SerializeField] private string idleAnimation = "Boss_Idle";
    [SerializeField] private string engageAnimation = "Boss_Engage";
    [SerializeField] private float engageAnimationDuration = 1.2f;
    [SerializeField] private string closeAttackWarningAnimation = "Boss_Close_Warn";
    [SerializeField] private float closeAttackWarningDuration = 0.45f;
    [SerializeField] private string closeAttackAnimation = "Boss_Close_Attack";
    [SerializeField] private float closeAttackHitDelay = 0.2f;
    [SerializeField] private float closeAttackRecovery = 0.55f;
    [SerializeField] private string bulletRainAnimation = "Boss_BulletRain";
    [SerializeField] private float bulletRainCastDuration = 1f;
    [SerializeField] private float bulletRainRecovery = 0.6f;
    [SerializeField] private string chargeAnimation = "Boss_Charge";
    [SerializeField] private float chargeWindupDuration = 0.8f;
    [SerializeField] private float chargeDuration = 0.35f;
    [SerializeField] private float chargeRecovery = 0.7f;
    [SerializeField] private string deathAnimation = "Boss_Death";
    [SerializeField] private float deathAnimationDuration = 1.6f;

    [Header("Attack Settings")]
    [SerializeField] private float closeAttackDamage = 28f;
    [SerializeField] private float meleeRepositionTolerance = 0.2f;
    [SerializeField] private float approachMoveTimeout = 2f;

    [SerializeField] private GameObject bulletRainProjectile;
    [SerializeField] private float bulletRainDamage = 14f;
    [SerializeField] private int bulletRainProjectileCount = 6;
    [SerializeField] private float bulletRainRadius = 2.5f;
    [SerializeField] private float bulletRainSpawnHeight = 4f;
    [SerializeField] private float bulletRainFallSpeed = 10f;
    [SerializeField] private float bulletRainProjectileLifetime = 3f;

    [SerializeField] private float chargeSpeed = 12f;
    [SerializeField] private float chargeDamage = 35f;
    [SerializeField] private float chargeHitRadius = 1.2f;

    [Header("Repositioning")]
    [SerializeField] private float combatRangeTolerance = 0.4f;
    [SerializeField] private float repositionTimeout = 2.5f;
    [SerializeField] private LayerMask playerLayer = ~0;

    [Header("Hitbox Settings")]
    [SerializeField] private float closeAttackHitboxRadius = 1.75f;
    [SerializeField] private Vector2 closeAttackHitboxOffset = new Vector2(1f, 0.1f);
    [SerializeField] private float closeAttackHitboxDuration = 0.25f;
    [SerializeField] private Vector2 chargeHitboxOffset = new Vector2(1.1f, 0f);
    [SerializeField] private float chargeHitboxExtraLifetime = 0.1f;

    private readonly Blackboard blackboard = new Blackboard();
    private AnimatorAdapter animatorAdapter;
    private StateMachine.StateMachine stateMachine;
    private BossEntryState entryState;
    private BossCombatState combatState;
    private BossDefeatedState defeatedState;

    private float currentHealth;
    private bool pendingCombatEntry;
    private BossAttackType lastAttack = BossAttackType.CloseRange;
    private Vector3 initialScale;
    private BossDamageHitbox closeAttackHitbox;
    private BossDamageHitbox chargeAttackHitbox;

    public float MoveSpeed => moveSpeed;
    public float AttackRange => meleeRange;
    public float AttackCooldown => attackCooldown;
    public Transform Player { get => playerTransform; set => playerTransform = value; }
    public Transform Self => transform;
    public Vector2 Position => body != null ? body.position : (Vector2)transform.position;
    public float Health
    {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, 0f, maxHealth);
    }

    public bool IsAlive => currentHealth > 0f;
    public IAnimator Animator => animatorAdapter;
    public Blackboard Blackboard => blackboard;
    public bool DebugBehaviour => debugBehaviour;

    public bool HasPlayer => playerTransform != null;
    public bool HasPendingCombatEntry => pendingCombatEntry;

    public float DetectionRange => detectionRange;
    public float CombatRange => combatRange;
    public float MeleeRange => meleeRange;
    public float MeleeRepositionTolerance => meleeRepositionTolerance;
    public float ApproachMoveTimeout => approachMoveTimeout;

    public string IdleAnimation => idleAnimation;
    public string EngageAnimation => engageAnimation;
    public float EngageAnimationDuration => engageAnimationDuration;
    public string CloseAttackWarningAnimation => closeAttackWarningAnimation;
    public float CloseAttackWarningDuration => closeAttackWarningDuration;
    public string CloseAttackAnimation => closeAttackAnimation;
    public float CloseAttackHitDelay => closeAttackHitDelay;
    public float CloseAttackRecovery => closeAttackRecovery;
    public float CloseAttackDamage => closeAttackDamage;
    public string BulletRainAnimation => bulletRainAnimation;
    public float BulletRainCastDuration => bulletRainCastDuration;
    public float BulletRainRecovery => bulletRainRecovery;
    public string ChargeAnimation => chargeAnimation;
    public float ChargeWindupDuration => chargeWindupDuration;
    public float ChargeDuration => chargeDuration;
    public float ChargeRecovery => chargeRecovery;
    public float ChargeDamage => chargeDamage;
    public float ChargeHitRadius => chargeHitRadius;
    public float ChargeSpeed => chargeSpeed;
    public float CloseAttackHitboxRadius => closeAttackHitboxRadius;
    public Vector2 CloseAttackHitboxOffset => closeAttackHitboxOffset;
    public float CloseAttackHitboxDuration => closeAttackHitboxDuration;
    public Vector2 ChargeHitboxOffset => chargeHitboxOffset;
    public float ChargeHitboxExtraLifetime => chargeHitboxExtraLifetime;
    public bool DebugEnabled => debugBehaviour;
    public Vector2 ForwardDirection => transform.localScale.x < 0f ? Vector2.left : Vector2.right;
    public LayerMask PlayerLayer => playerLayer;

    private void Awake()
    {
        if (body == null)
        {
            body = GetComponent<Rigidbody2D>();
        }

        animatorAdapter = new AnimatorAdapter(animator);
        currentHealth = maxHealth;
        initialScale = transform.localScale;
    }

    private void Start()
    {
        if (!HasPlayer)
        {
            TryFindPlayerReference();
        }

        stateMachine = new StateMachine.StateMachine();
        entryState = new BossEntryState(this, Animator);
        combatState = new BossCombatState(this, Animator);
        defeatedState = new BossDefeatedState(this, Animator);

        stateMachine.SetState(entryState);
        stateMachine.AddTransition(entryState, combatState, new FuncPredicate(TryConsumeCombatEntryRequest));
        stateMachine.AddTransition(entryState, defeatedState, new FuncPredicate(() => !IsAlive));
        stateMachine.AddTransition(combatState, defeatedState, new FuncPredicate(() => !IsAlive));
    }

    private void Update()
    {
        stateMachine?.Update();
    }

    private void FixedUpdate()
    {
        stateMachine?.FixedUpdate();
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;

        currentHealth -= Mathf.Abs(damage);
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            StopAllCoroutines();
            StopMovement();
        }
    }

    public void ResetCombatEntryRequest()
    {
        pendingCombatEntry = false;
    }

    public void QueueCombatEntry()
    {
        pendingCombatEntry = true;
    }

    private bool TryConsumeCombatEntryRequest()
    {
        if (!pendingCombatEntry)
            return false;

        pendingCombatEntry = false;
        return true;
    }

    public bool IsPlayerWithinDetectionRange(float overrideRange = -1f)
    {
        if (!HasPlayer)
            return false;

        float range = overrideRange > 0f ? overrideRange : detectionRange;
        return Vector2.Distance(transform.position, playerTransform.position) <= range;
    }

    public bool IsPlayerWithinMeleeRange()
    {
        return IsPlayerWithinRadius(meleeRange);
    }

    public float DistanceToPlayer()
    {
        if (!HasPlayer)
            return float.PositiveInfinity;

        return Vector2.Distance(transform.position, playerTransform.position);
    }

    public void TryFindPlayerReference()
    {
        GameObject found = GameObject.FindGameObjectWithTag("Player");
        if (found != null)
        {
            playerTransform = found.transform;
        }
    }

    public void StopMovement()
    {
        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
        }
    }

    public void FacePlayer()
    {
        if (!HasPlayer)
            return;

        Vector3 scale = initialScale;
        bool faceLeft = playerTransform.position.x < transform.position.x;
        scale.x = Mathf.Abs(scale.x) * (faceLeft ? -1f : 1f);
        transform.localScale = scale;
    }

    public IEnumerator MoveToDistanceRoutine(float targetDistance, float tolerance, float timeout)
    {
        if (!HasPlayer)
            yield break;

        float elapsed = 0f;
        targetDistance = Mathf.Max(0f, targetDistance);
        tolerance = Mathf.Max(0.01f, tolerance);
        timeout = Mathf.Max(Time.deltaTime, timeout);

        while (elapsed < timeout)
        {
            if (!HasPlayer)
                yield break;

            Vector2 toPlayer = playerTransform.position - transform.position;
            float distance = toPlayer.magnitude;
            if (Mathf.Abs(distance - targetDistance) <= tolerance)
                break;

            Vector2 desiredPosition = (Vector2)playerTransform.position - toPlayer.normalized * targetDistance;
            Vector2 currentPos = body != null ? body.position : (Vector2)transform.position;
            Vector2 next = Vector2.MoveTowards(currentPos, desiredPosition, moveSpeed * Time.deltaTime);

            if (body != null)
            {
                body.MovePosition(next);
            }
            else
            {
                transform.position = next;
            }

            FacePlayer();
            elapsed += Time.deltaTime;
            yield return null;
        }

        StopMovement();
    }

    public IEnumerator RepositionToCombatRangeRoutine()
    {
        if (!HasPlayer)
            yield break;

        yield return MoveToDistanceRoutine(combatRange, combatRangeTolerance, repositionTimeout);
    }

    public BossAttackType SelectRandomAttack()
    {
        int attempts = 0;
        BossAttackType selected = lastAttack;
        while (attempts < 4)
        {
            selected = (BossAttackType)Random.Range(0, 3);
            if (selected != lastAttack)
                break;
            attempts++;
        }

        lastAttack = selected;
        return selected;
    }

    public void SpawnBulletRain()
    {
        if (bulletRainProjectile == null || !HasPlayer)
            return;

        Vector3 playerPos = playerTransform.position;
        for (int i = 0; i < Mathf.Max(1, bulletRainProjectileCount); i++)
        {
            Vector2 offset = Random.insideUnitCircle * bulletRainRadius;
            Vector3 spawn = new Vector3(playerPos.x + offset.x, playerPos.y + bulletRainSpawnHeight, playerPos.z);
            BulletPool.Spawn(
                bulletRainProjectile,
                spawn,
                Quaternion.identity,
                Vector2.down,
                bulletRainFallSpeed,
                bulletRainDamage,
                bulletRainProjectileLifetime,
                gameObject);
        }
    }



    public IEnumerator PerformChargeAttack()
    {
        Vector2 direction = Vector2.right;
        if (HasPlayer)
        {
            direction = (playerTransform.position - transform.position).normalized;
            if (direction.sqrMagnitude < Mathf.Epsilon)
            {
                direction = Vector2.right * Mathf.Sign(transform.localScale.x == 0f ? 1f : transform.localScale.x);
            }
        }
        else
        {
            direction = transform.localScale.x < 0f ? Vector2.left : Vector2.right;
        }

        float dashDuration = Mathf.Max(0f, chargeDuration);
        float extraLifetime = Mathf.Max(0f, chargeHitboxExtraLifetime);
        BossDamageHitbox hitbox = EnsureHitbox(ref chargeAttackHitbox, "Boss_ChargeHitbox");
        hitbox.Activate(this, chargeDamage, chargeHitRadius, chargeHitboxOffset, dashDuration + extraLifetime, true);

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            if (body != null)
            {
                body.linearVelocity = direction * chargeSpeed;
            }
            else
            {
                transform.position += (Vector3)(direction * chargeSpeed * Time.deltaTime);
            }

            FaceDirection(direction);

            elapsed += Time.deltaTime;
            yield return null;
        }

        StopMovement();
    }

    public IEnumerator ActivateCloseRangeHitbox()
    {
        float lifetime = Mathf.Max(0f, closeAttackHitboxDuration);
        BossDamageHitbox hitbox = EnsureHitbox(ref closeAttackHitbox, "Boss_CloseAttackHitbox");
        hitbox.Activate(this, closeAttackDamage, closeAttackHitboxRadius, closeAttackHitboxOffset, lifetime, true);

        if (lifetime > 0f)
        {
            yield return new WaitForSeconds(lifetime);
        }
        else
        {
            yield return null;
        }

        hitbox.Deactivate();
    }

    public bool ApplyDamageToPlayer(float damage)
    {
        if (!HasPlayer)
            return false;

        return ApplyDamageToTarget(playerTransform, damage);
    }

    public bool DamagePlayerWithinRadius(Vector2 center, float radius, float damage)
    {
        if (!HasPlayer)
            return false;

        float clampedRadius = Mathf.Max(0f, radius);
        if (clampedRadius <= Mathf.Epsilon)
            return false;

        var hits = Physics2D.OverlapCircleAll(center, clampedRadius, playerLayer);
        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];
            if (hit == null)
                continue;

            Transform target = hit.transform;
            if (target == null)
                continue;

            if (target == playerTransform || target.IsChildOf(playerTransform))
            {
                return ApplyDamageToTarget(target, damage);
            }
        }

        if (IsPlayerWithinRadius(clampedRadius))
        {
            return ApplyDamageToPlayer(damage);
        }

        return false;
    }

    private bool ApplyDamageToTarget(Transform target, float damage)
    {
        if (target == null)
            return false;

        int dmg = Mathf.RoundToInt(Mathf.Abs(damage));
        bool applied = false;

        var status = target.GetComponentInParent<PlayerStatus>();
        if (status != null)
        {
            status.TakeDamage(dmg);
            applied = true;
        }
        else
        {
            target.gameObject.SendMessage("TakeDamage", dmg, SendMessageOptions.DontRequireReceiver);
            applied = true;
        }

        return applied;
    }

    private bool IsPlayerWithinRadius(float radius)
    {
        if (!HasPlayer)
            return false;

        return Vector2.Distance(transform.position, playerTransform.position) <= Mathf.Max(0f, radius);
    }

    private void FaceDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude <= Mathf.Epsilon)
            return;

        Vector3 scale = initialScale;
        scale.x = Mathf.Abs(scale.x) * (dir.x < 0f ? -1f : 1f);
        transform.localScale = scale;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        StopMovement();
        closeAttackHitbox?.Deactivate();
        chargeAttackHitbox?.Deactivate();
    }

    private BossDamageHitbox EnsureHitbox(ref BossDamageHitbox cache, string name)
    {
        if (cache != null)
            return cache;

        GameObject hitboxObject = new GameObject(name);
        hitboxObject.SetActive(false);
        hitboxObject.transform.SetParent(transform, false);
        hitboxObject.transform.localPosition = Vector3.zero;
        hitboxObject.transform.localRotation = Quaternion.identity;
        hitboxObject.transform.localScale = Vector3.one;
        hitboxObject.layer = gameObject.layer;
        cache = hitboxObject.AddComponent<BossDamageHitbox>();
        return cache;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, combatRange);
    }

    private class BossDefeatedState : IState
    {
        private readonly BossController boss;
        private readonly IAnimator animator;
        private Coroutine defeatRoutine;

        public BossDefeatedState(BossController boss, IAnimator animator)
        {
            this.boss = boss;
            this.animator = animator;
        }

        public void onEnter()
        {
            boss.StopMovement();
            defeatRoutine = boss.StartCoroutine(HandleDefeat());
        }

        public void onExit()
        {
            if (defeatRoutine != null)
            {
                boss.StopCoroutine(defeatRoutine);
                defeatRoutine = null;
            }
        }

        public void onUpdate()
        {
        }

        public void onFixedUpdate()
        {
        }

        private IEnumerator HandleDefeat()
        {
            if (!string.IsNullOrEmpty(boss.deathAnimation))
            {
                animator.Play(boss.deathAnimation);
            }

            float duration = Mathf.Max(0f, boss.deathAnimationDuration);
            if (duration > 0f)
                yield return new WaitForSeconds(duration);

            boss.gameObject.SetActive(false);
            defeatRoutine = null;
        }
    }
}

