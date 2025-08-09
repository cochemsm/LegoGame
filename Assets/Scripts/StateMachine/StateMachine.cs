using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace StateMachine {
    public class StateMachine {
        public State CurrentState { get; private set; }

        private readonly Dictionary<State, List<Transition>> _availableState = new Dictionary<State, List<Transition>>();
        private readonly List<Transition> _anyTransitions = new List<Transition>();
        
        public void SetState(State state) {
            if (!_availableState.Keys.ToListPooled().Contains(state)) return;
            
            CurrentState?.Exit();
            CurrentState = state;
            CurrentState.Enter();
        }

        public void AddState(State state) {
            if (!_availableState.Keys.ToListPooled().Contains(state)) return;
            
            _availableState.Add(state, new List<Transition>());
        }

        public void TransitionFromStateToState(State fromState, State toState, Func<bool> condition) {
            if (!_availableState.Keys.ToListPooled().Contains(fromState)) return;
            if (!_availableState.Keys.ToListPooled().Contains(toState)) return;
            
            _availableState[fromState].Add(new Transition{ ToState = toState, Condition = condition });
        }

        public void TransitionFromAnyToState(State toState, Func<bool> condition) {
            if (!_availableState.Keys.ToListPooled().Contains(toState)) return;
            
            _anyTransitions.Add(new Transition{ ToState = toState, Condition = condition });
        }
        
        public void Update() {
            CurrentState.Update();

            foreach (var transition in _availableState[CurrentState]) {
                if (transition.Condition.Invoke()) {
                    SetState(transition.ToState);
                }
            }
            
            foreach (var transition in _anyTransitions) {
                if (transition.Condition.Invoke()) {
                    SetState(transition.ToState);
                }
            }
        }
    }
}
