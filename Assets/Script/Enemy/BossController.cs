using UnityEngine;

public class BossController : MonoBehaviour, IEnemy
{
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private Transform player;
    [SerializeField] private float health = 300f;
    [SerializeField] private Animator animator; // added so states can receive animator

    // Add a field for Blackboard
    private Blackboard blackboard;
    public Blackboard Blackboard => blackboard;

    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    public Transform Player => player;
    public Transform Self => transform;
    public bool IsAlive => health > 0;
    public float Health { get => health; set => health = value; }

    // Implement IEnemy.Animator
    public IAnimator Animator => animator as IAnimator;

    Transform IEnemy.Player { get => Player; set => throw new System.NotImplementedException(); }

    private StateMachine.StateMachine stateMachine;
    private EnemyIdleState idleState;
    private EnemyCombatState combatState;
    private EnemyDieState dieState;

    private void Start()
    {
        // Initialize blackboard
        blackboard = new Blackboard();

        //stateMachine = new StateMachine.StateMachine();
        //idleState = new EnemyIdleState(this, animator);
        //combatState = new EnemyCombatState(this, animator);
        //dieState = new EnemyDieState(this, animator);

        stateMachine.SetState(idleState);

        stateMachine.AddTransition(idleState, combatState, new StateMachine.FuncPredicate(() => Vector3.Distance(transform.position, player.position) <= attackRange));
        stateMachine.AddTransition(combatState, idleState, new StateMachine.FuncPredicate(() => Vector3.Distance(transform.position, player.position) > attackRange));
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