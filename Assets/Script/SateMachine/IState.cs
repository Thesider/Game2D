namespace StateMachine
{
    public interface IState
    {
        void onEnter();
        void onExit();
        void onUpdate();
        void onFixedUpdate();
    }
}