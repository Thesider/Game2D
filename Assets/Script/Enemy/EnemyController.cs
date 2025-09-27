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

    // Per-enemy runtime data
    private readonly Blackboard blackboard = new Blackboard();
    private AnimatorAdapter animatorAdapter;

    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    public Transform Player => player;
    public Transform Self => transform;

    // Projectile accessors
    public GameObject BulletPrefab => bulletPrefab;
    public Transform AttackPoint => attackPoint;
    public float AttackDamage => attackDamage;
    public float BulletSpeed => bulletSpeed;
    public float BulletLifetime => bulletLifetime;

    public float Health { get => health; set => health = value; }

    // IEnemy interface wiring
    public IEnemyAnimator Animator => animatorAdapter;
    public Blackboard Blackboard => blackboard;

    private StateMachine.StateMachine stateMachine;
    private EnemyIdleState idleState;
    private EnemyCombatState combatState;
    private EnemyDieState dieState;

    private void Awake()
    {
        animatorAdapter = new AnimatorAdapter(animator);
    }

    private void OnEnable()
    {
        // Record spawn time so behaviour nodes can avoid triggering attack animations immediately after spawn.
        // OnEnable runs when the pooled enemy is activated by the spawner.
        blackboard.Set("spawnTime", Time.time);
    }

    private void Start()
    {
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
}
