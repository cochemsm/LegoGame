using System;

namespace CustomStateMachine {
    public struct Transition {
        public State ToState;
        public Func<bool> Condition;
    }
}
