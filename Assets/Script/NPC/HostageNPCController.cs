using UnityEngine;

public class HostageNPCController : MonoBehaviour, INPC
{
    [Header("Identity")]
    [SerializeField] private string npcId = "hostage_01";
    [SerializeField] private NPCType type = NPCType.Hostage;

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator animator;

    [Header("Stats")]
    [SerializeField] private float health = 50f;

    private readonly Blackboard blackboard = new Blackboard();
    private AnimatorAdapter animatorAdapter;

    private StateMachine.StateMachine stateMachine;
    private NPCCapturedState capturedState;
    private NPCRescuedState rescuedState;
    private NPCFleeingState fleeingState;

    public string NPCId => npcId;
    public NPCType Type => type;
    public Transform Transform => transform;
    public Transform PlayerTransform => playerTransform;
    public bool IsInteractable => true;
    public bool IsAlive => health > 0f;
    public Blackboard Blackboard => blackboard;
    public IAnimator Animator => animatorAdapter;


    private void Awake()
    {
        animatorAdapter = new AnimatorAdapter(animator);
    }

    private void Start()
    {
        // create states
        capturedState = new NPCCapturedState(this);
        rescuedState = new NPCRescuedState(this);
        fleeingState = new NPCFleeingState(this);

        stateMachine = new StateMachine.StateMachine();

        // start as captured
        stateMachine.SetState(capturedState);

        // transitions: captured -> rescued when player interacts (we set a blackboard flag in OnInteract)
        stateMachine.AddTransition(capturedState, rescuedState, new StateMachine.FuncPredicate(() => blackboard.TryGet("IsRescued", out bool v) && v));

        // rescued -> fleeing after recovered
        stateMachine.AddTransition(rescuedState, fleeingState, new StateMachine.FuncPredicate(() => rescuedState.IsRecovered()));

        // any -> null or cleanup on death
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
        // Player rescues hostage
        blackboard.Set("IsRescued", true);
        animatorAdapter.SetTrigger("ThankPlayer");
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
        // simple destroy after animation (optional hook)
        Destroy(gameObject, 3f);
    }

    public void UpdateNPC()
    {
        // keep compatibility with possible external calls
        stateMachine.Update();
    }
}
