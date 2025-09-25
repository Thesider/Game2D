using System;
using System.Collections.Generic;

namespace StateMachine
{
    public class StateMachine
    {
        StateNode currentState;
        Dictionary<Type, StateNode> states = new();
        HashSet<ITransition> anyTransitions = new();

        public void Update()
        {
            var transition = GetTransition();
            if (transition != null)
            {
                ChangeState(transition.To);
            }
            currentState.State.onUpdate();
        }

        public void FixedUpdate()
        {
            currentState.State.onFixedUpdate();
        }

        public void SetState(IState state)
        {
            currentState = states[state.GetType()];
            currentState.State.onEnter();
        }

        public void ChangeState(IState state)
        {
            if (state == currentState)
            {
                return;
            }
            var PreviousState = currentState.State;
        }

        ITransition GetTransition()
        {
            foreach (var transition in anyTransitions)
            {
                if (transition.Condition.Evaluate())
                {
                    return transition;
                }
            }
            foreach (var transition in currentState.Transitions)
            {
                if (transition.Condition.Evaluate())
                {
                    return transition;
                }
            }
            return null;
        }

        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }

        StateNode GetOrAddNode(IState state)
        {
            var node = states.GetValueOrDefault(state.GetType());
            if (node == null)
            {
                node = new StateNode(state);
                states.Add(state.GetType(), node);
            }
            return node;
        }
        class StateNode
        {
            public IState State;
            public HashSet<ITransition> Transitions { get; }
            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<ITransition>();
            }
            public void AddTransition(IState to, IPredicate condition)
            {
                Transitions.Add(new Transition(to, condition));

            }
        }


    }
}
