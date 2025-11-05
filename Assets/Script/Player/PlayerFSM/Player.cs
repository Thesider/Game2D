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


    [SerializeField] private PlayerData playerData;

    #endregion

    #region Components
    public Animator anim { get; private set; }
    public PlayerInputHandler inputHandler { get; private set; }
    public Rigidbody2D rb { get; private set; }
    #endregion

    #region Check Transforms Variables
    [SerializeField] private Transform groundCheck;


    #endregion

    #region Other Variables
    public Vector2 currentVelocity { get; private set; }
    public int facingDirection { get; private set; } 

    private Vector2 workspace;
    #endregion

    #region Unity Callbacks Functions
    public void Awake() {
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, playerData, "idle");
        moveState = new PlayerMoveState(this, stateMachine, playerData, "move");
        jumpState = new PLayerJumpState(this, stateMachine, playerData, "inAir");
        inAirState = new PlayerInAirState(this, stateMachine, playerData, "inAir");
        landState = new PlayerLandState(this, stateMachine, playerData, "land");
        dashState = new PlayerDashState(this, stateMachine, playerData, "dash");
    }

    private void Start() { 
        anim = GetComponent<Animator>();
        inputHandler = GetComponent<PlayerInputHandler>();
        rb = GetComponent<Rigidbody2D>();

        facingDirection = 1;

        stateMachine.Initialize(idleState);
    }

    private void Update() {
        currentVelocity = rb.linearVelocity;

        stateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate() {
        stateMachine.CurrentState.PhysicsUpdate();
    
    }
    #endregion

    #region Set Functions
    //set velocity
    //public void SetVelocity(Vector2 velocity) {
    //    rb.linearVelocity = velocity;
    //    currentVelocity = velocity;
    //}
    public void SetVelocityX(float velocity) {
        workspace.Set(velocity, currentVelocity.y);
        rb.linearVelocity = workspace;
        currentVelocity = workspace;
    }

    public void SetVelocityY(float velocity) {
        workspace.Set(currentVelocity.x, velocity);
        rb.linearVelocity = workspace;
        currentVelocity = workspace;
    }
    #endregion

    #region Check Functions

    public bool CheckIfGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);
    }

    //flip character
    public void CheckFlip(int xInput) {
        if (xInput != 0 && xInput != facingDirection) {
            Flip();
        }
    }
    #endregion

    #region Other Functions
    private void AnimationTrigger() => stateMachine.CurrentState.AnimationTrigger();
    private void AnimationFinishTrigger() => stateMachine.CurrentState.AnimationFinishTrigger();
    private void Flip() {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);

    }
    #endregion

}
