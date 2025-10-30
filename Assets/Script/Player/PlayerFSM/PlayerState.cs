using UnityEngine;

public class PlayerState : MonoBehaviour {
    
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;

    protected bool isAnimationFinished;

    protected float startTime;

    private string animBoolName;

    public PlayerState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) {
        this.player = player;
        this.stateMachine = stateMachine;
        this.playerData = playerData;
        this.animBoolName = animBoolName;
    }   

    public virtual void Enter() {
        Dochecks();
        player.anim.SetBool(animBoolName, true);
        startTime = Time.time;
        Debug.Log($"Enter State: {animBoolName}");
        isAnimationFinished = false;
    }

    public virtual void Exit() {
        player.anim.SetBool(animBoolName, false);

    }

    public virtual void LogicUpdate() {

    }
    public virtual void PhysicsUpdate() {
        Dochecks();

    }

    public virtual void Dochecks() {
         
    }
    public virtual void AnimationTrigger() { 
        
    }
    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;
}
