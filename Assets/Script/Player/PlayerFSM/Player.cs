using UnityEngine;

public class Player : MonoBehaviour{
    #region State Variables
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PLayerJumpState jumpState { get; private set; }
    public PlayerInAirState inAirState { get; private set; }
    public PlayerLandState landState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerCrouchIdleState crouchIdleState { get; private set; }
    public PlayerCrouchMoveState crouchMoveState { get; private set; }
    public PlayerAttackState primaryAttackState { get; private set; }
    public PlayerAttackState secondAttackState { get; private set; }

    [SerializeField] private PlayerData playerData;

    #endregion

    #region Components
    public Core core { get; private set; }  
    public Animator anim { get; private set; }
    public PlayerInputHandler inputHandler { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public BoxCollider2D movementCollider { get; private set; }
    public PlayerInventory playerInventory { get; private set; }

    #endregion

    #region Other Variables
    //public Vector2 currentVelocity { get; private set; }




    private Vector2 workspace;

    #endregion

    #region Unity Callbacks Functions
    public void Awake() {

        core = GetComponentInChildren<Core>();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, playerData, "idle");
        moveState = new PlayerMoveState(this, stateMachine, playerData, "move");
        jumpState = new PLayerJumpState(this, stateMachine, playerData, "inAir");
        inAirState = new PlayerInAirState(this, stateMachine, playerData, "inAir");
        landState = new PlayerLandState(this, stateMachine, playerData, "land");
        dashState = new PlayerDashState(this, stateMachine, playerData, "dash");
        crouchIdleState = new PlayerCrouchIdleState(this, stateMachine, playerData, "crouchIdle");
        crouchMoveState = new PlayerCrouchMoveState(this, stateMachine, playerData, "crouchMove");
        primaryAttackState = new PlayerAttackState(this, stateMachine, playerData, "attack");
        secondAttackState = new PlayerAttackState(this, stateMachine, playerData, "attack");
    }

    private void Start() { 
        anim = GetComponent<Animator>();
        inputHandler = GetComponent<PlayerInputHandler>();
        rb = GetComponent<Rigidbody2D>();
        movementCollider = GetComponent<BoxCollider2D>();
        playerInventory = GetComponent<PlayerInventory>();

        primaryAttackState.SetWeapon(playerInventory.weapons[(int)CombatInputs.primary]);
        //secondAttackState.SetWeapon(playerInventory.weapons[(int)CombatInputs.secondary]);

        stateMachine.Initialize(idleState);
    }

    private void Update() {
        core.LogicUpdate();
        stateMachine.CurrentState.LogicUpdate();                                    
    }

    private void FixedUpdate() {
        stateMachine.CurrentState.PhysicsUpdate();
    
    }
    #endregion

    



    #region Other Functions
    public void SetColliderHeight(float height) {
        Vector2 center = movementCollider.offset;
        workspace.Set(movementCollider.size.x, height);

        center.y += (height - movementCollider.size.y) / 2;

        movementCollider.size = workspace;
        movementCollider.offset = center;
    }

    private void AnimationTrigger() => stateMachine.CurrentState.AnimationTrigger();
    private void AnimationFinishTrigger() => stateMachine.CurrentState.AnimationFinishTrigger();

    #endregion

}
