using System;

namespace StateMachine {
    public struct Transition {
        public State ToState;
        public Func<bool> Condition;
    }
}
