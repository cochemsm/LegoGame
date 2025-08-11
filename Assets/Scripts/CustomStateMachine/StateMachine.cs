using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CustomStateMachine {
    public class StateMachine {
        public State CurrentState { get; private set; }

        private readonly Dictionary<State, List<Transition>> _availableState = new Dictionary<State, List<Transition>>();
        private readonly List<Transition> _anyTransitions = new List<Transition>();
        
        public void SetState(State state) {
            CurrentState?.Exit();
            CurrentState = state;
            CurrentState.Enter();
        }

        public void AddState(State state) {
            _availableState.Add(state, new List<Transition>());
        }

        public void TransitionFromStateToState(State fromState, State toState, Func<bool> condition) {
            _availableState[fromState].Add(new Transition{ ToState = toState, Condition = condition });
        }

        public void TransitionFromAnyToState(State toState, Func<bool> condition) {
            _anyTransitions.Add(new Transition{ ToState = toState, Condition = condition });
        }
        
        public void Update() {
            if (CurrentState == null) return;
            
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
