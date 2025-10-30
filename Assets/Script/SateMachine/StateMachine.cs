using System;
using System.Collections.Generic;

namespace StateMachine
{
    public class StateMachine
    {
        StateNode currentState;
        Dictionary<Type, StateNode> states = new Dictionary<Type, StateNode>();
        List<ITransition> anyTransitions = new List<ITransition>();

        public void Update()
        {
            if (currentState == null)
            {
                var pending = GetTransition();
                if (pending != null)
                {
                    ApplyTransition(pending);
                }

                if (currentState == null) return;
            }

            var transition = GetTransition();
            if (transition != null)
            {
                ApplyTransition(transition);
                if (currentState == null) return;
            }

            currentState.State.onUpdate();
        }

        public void FixedUpdate()
        {
            if (currentState == null) return;
            currentState.State.onFixedUpdate();
        }

        public void SetState(IState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            var node = GetOrAddNode(state);

            if (currentState != null)
            {
                currentState.State.onExit();
            }

            currentState = node;
            currentState.State.onEnter();
        }

        public void ChangeState(IState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            var node = GetOrAddNode(state);
            if (node == currentState) return;

            if (currentState != null)
                currentState.State.onExit();

            currentState = node;
            currentState.State.onEnter();
        }

        ITransition GetTransition()
        {
            foreach (var transition in anyTransitions)
            {
                if (transition?.Condition != null && transition.Condition.Evaluate())
                {
                    return transition;
                }
            }

            if (currentState == null) return null;

            foreach (var transition in currentState.Transitions)
            {
                if (transition?.Condition != null && transition.Condition.Evaluate())
                {
                    return transition;
                }
            }
            return null;
        }


        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            if (to == null) throw new ArgumentNullException(nameof(to));
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            if (from == null)
            {
                anyTransitions.Add(new Transition(to, condition));
                return;
            }

            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }


        public void AddAnyTransition(IState to, IPredicate condition)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            anyTransitions.Add(new Transition(to, condition));
        }

        StateNode GetOrAddNode(IState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            var key = state.GetType();
            if (!states.TryGetValue(key, out var node))
            {
                node = new StateNode(state);
                states.Add(key, node);
            }
            return node;
        }

        void ApplyTransition(ITransition transition)
        {
            if (transition == null) return;

            if (transition.To != null)
            {
                ChangeState(transition.To);
                return;
            }

            ExitCurrentState();
        }

        void ExitCurrentState()
        {
            if (currentState == null) return;

            currentState.State.onExit();
            currentState = null;
        }

        class StateNode
        {
            public IState State;
            public List<ITransition> Transitions { get; }

            public StateNode(IState state)
            {
                State = state;
                Transitions = new List<ITransition>();
            }

            public void AddTransition(IState to, IPredicate condition)
            {
                Transitions.Add(new Transition(to, condition));
            }
        }
    }
}
