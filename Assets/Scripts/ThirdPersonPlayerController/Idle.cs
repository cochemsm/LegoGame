using CustomStateMachine;
using UnityEngine;

namespace ThirdPersonPlayerController {
    public class Idle : State {
        private Animator _animator;

        public Idle(Animator animator) {
            _animator = animator;
        }

        public override void Enter() {
            _animator.Play("Idle");
        }
    }
}
