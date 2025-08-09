using CustomStateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ThirdPersonPlayerController {
    public class Walking : State {
        private Rigidbody _rigidbody;
        private readonly InputAction _moveAction;
        private float _movementSpeed;

        public Walking(Rigidbody rigidbody, InputAction moveAction, float movementSpeed) {
            _rigidbody = rigidbody;
            _moveAction = moveAction;
            _movementSpeed = movementSpeed;
        }

        public override void Update() {
            Vector2 input = _moveAction.ReadValue<Vector2>() * (_movementSpeed * Time.deltaTime);
        }
    }
}
