using UnityEngine;

public class EnemyController : MonoBehaviour, IEnemy
{
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private Transform player;
    [SerializeField] private float health = 100f;

    [SerializeField] private Animator animator;
    [Header("Projectile")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float bulletSpeed = 8f;
    [SerializeField] private float bulletLifetime = 4f;

    private readonly Blackboard blackboard = new Blackboard();
    private AnimatorAdapter animatorAdapter;

    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    public Transform Player { get => player; set => player = value; }

    // Convenience setter if other code prefers a method call
    public void SetPlayer(Transform t)
    {
        player = t;
    }
    public Transform Self => transform;

    public bool IsAlive => health > 0;

    public GameObject BulletPrefab => bulletPrefab;
    public Transform AttackPoint => attackPoint;
    public float AttackDamage => attackDamage;
    public float BulletSpeed => bulletSpeed;
    public float BulletLifetime => bulletLifetime;

    public float Health { get => health; set => health = value; }

    // IEnemy interface wiring (expose generic animator facade)
    public IAnimator Animator => animatorAdapter;
    public Blackboard Blackboard => blackboard;

    private StateMachine.StateMachine stateMachine;
    private EnemyIdleState idleState;
    private EnemyCombatState combatState;
    private EnemyDieState dieState;

    private void Awake()
    {
        animatorAdapter = new AnimatorAdapter(animator);
        // store the original serialized health so we can reset when reusing pooled instances
        initialHealth = health;
    }

    private void OnEnable()
    {

        blackboard.Set("spawnTime", Time.time);
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
            else
                Debug.LogWarning("EnemyController: Player not found in scene!");
        }
        stateMachine = new StateMachine.StateMachine();
        idleState = new EnemyIdleState(this, Animator);
        combatState = new EnemyCombatState(this, Animator);
        dieState = new EnemyDieState(this, Animator);

        stateMachine.SetState(idleState);

        stateMachine.AddTransition(idleState, combatState, new StateMachine.FuncPredicate(() => Vector3.Distance(transform.position, player.position) <= detectionRange));
        stateMachine.AddTransition(combatState, idleState, new StateMachine.FuncPredicate(() => Vector3.Distance(transform.position, player.position) > detectionRange));
        stateMachine.AddTransition(idleState, dieState, new StateMachine.FuncPredicate(() => health <= 0));
        stateMachine.AddTransition(combatState, dieState, new StateMachine.FuncPredicate(() => health <= 0));
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        if (player == null) return;
        float distance = Vector2.Distance(transform.position, player.position);
        stateMachine.FixedUpdate();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            stateMachine.SetState(dieState);
        }
    }

    public void OnDieAnimationComplete()
    {

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        blackboard.Clear();
    }

    public void ResetForReuse()
    {
        health = initialHealth;
        blackboard.Clear();
        // Re-enable colliders when returning to pool / reuse
        try
        {
            foreach (var c2 in GetComponentsInChildren<Collider2D>(true))
                c2.enabled = true;
            foreach (var c in GetComponentsInChildren<Collider>(true))
                c.enabled = true;
        }
        catch { }
    }

    private float initialHealth;
}
