using UnityEngine;

public class HelperNPCController : MonoBehaviour, INPC
{
    [Header("Identity")]
    [SerializeField] private string npcId = "helper_01";
    [SerializeField] private NPCType type = NPCType.Helper;

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator animator;

    [Header("Stats")]
    [SerializeField] private float health = 80f;

    private readonly Blackboard blackboard = new Blackboard();
    private AnimatorAdapter animatorAdapter;

    private StateMachine.StateMachine stateMachine;
    private NPCIdleState idleState;
    private NPCFollowingState followingState;
    private NPCCombatState combatState;

    #region INPC Implementation
    public string NPCId => npcId;
    public NPCType Type => type;
    public Transform Transform => transform;
    public Transform PlayerTransform => playerTransform;
    public bool IsInteractable => true;
    public bool IsAlive => health > 0f;
    public Blackboard Blackboard => blackboard;
    public IAnimator Animator => animatorAdapter;
    #endregion

    private void Awake()
    {
        animatorAdapter = new AnimatorAdapter(animator);
    }

    private void Start()
    {
        // create states
        idleState = new NPCIdleState(this);
        followingState = new NPCFollowingState(this);
        combatState = new NPCCombatState(this);

        stateMachine = new StateMachine.StateMachine();

        // default to idle
        stateMachine.SetState(idleState);

        // transitions
        // idle -> follow when player is nearby
        stateMachine.AddTransition(idleState, followingState, new StateMachine.FuncPredicate(() => Vector3.Distance(transform.position, playerTransform != null ? playerTransform.position : transform.position) <= 6f));

        // follow -> combat when enemies nearby (driven by behaviour tree/blackboard)
        stateMachine.AddTransition(followingState, combatState, new StateMachine.FuncPredicate(() => blackboard.TryGet("EnemiesNearby", out int count) && count > 0));

        // combat -> idle when no enemies
        stateMachine.AddTransition(combatState, idleState, new StateMachine.FuncPredicate(() => !blackboard.TryGet("EnemiesNearby", out int c) || c == 0));

        // any -> cleanup on death
        stateMachine.AddAnyTransition(null, new StateMachine.FuncPredicate(() => health <= 0f));
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void OnInteract(Transform player)
    {
        // Could toggle follow/assist
        blackboard.Set("FollowRequested", true);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f) Die();
    }

    public void Die()
    {
        health = 0f;
        animatorAdapter.SetTrigger("Die");
        Destroy(gameObject, 3f);
    }

    public void UpdateNPC()
    {
        stateMachine.Update();
    }
}
