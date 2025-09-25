namespace StateMachine
{
    public abstract class BaseState : IState
    {

        public virtual void onEnter() { }
        public virtual void onExit() { }
        public virtual void onUpdate() { }
        public virtual void onFixedUpdate() { }
    }

}
