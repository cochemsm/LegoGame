using CustomStateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ThirdPersonPlayerController {
    public class Falling : State {
        private Rigidbody _rigidbody;
        private float _fallingSpeed;
        private InputAction _moveAction;
        private float _movementSpeed;

        public Falling(Rigidbody rigidbody, float fallingSpeed, InputAction moveAction, float movementSpeed) {
            _rigidbody = rigidbody;
            _fallingSpeed = fallingSpeed;
            _moveAction = moveAction;
            _movementSpeed = movementSpeed;
        }
        
        public override void Update() {
            Vector2 input = _moveAction.ReadValue<Vector2>() * (_movementSpeed * Time.deltaTime);
            
            // Cam Oriented Movement
            var cam = RoomManager.currentCamera;
            Vector3 forward = cam.transform.forward;
            Vector3 right = cam.transform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();
            
            Vector3 inputDirection = input.x * right + input.y * forward;
            inputDirection += new Vector3(0, _rigidbody.linearVelocity.y, 0);
            inputDirection += Vector3.down * (Time.deltaTime * _fallingSpeed);
            
            _rigidbody.linearVelocity = inputDirection;
            
            // Look into Walking Direction
            if (input == Vector2.zero) return;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(inputDirection.x, 0, inputDirection.z), Vector3.up);
            _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, targetRotation, 10f * Time.fixedDeltaTime));
        }

        public override void Exit() => _rigidbody.linearVelocity = Vector3.zero;
    }
}
