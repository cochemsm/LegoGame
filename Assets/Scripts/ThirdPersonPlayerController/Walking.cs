using CustomStateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ThirdPersonPlayerController {
    public class Walking : State {
        private Rigidbody _rigidbody;
        private readonly InputAction _moveAction;
        private float _movementSpeed;
        private SphereCaster _groundCheck;

        public Walking(Rigidbody rigidbody, InputAction moveAction, float movementSpeed, SphereCaster groundCheck) {
            _rigidbody = rigidbody;
            _moveAction = moveAction;
            _movementSpeed = movementSpeed;
            _groundCheck = groundCheck;
        }

        public override void Update() {
            Vector2 input = _moveAction.ReadValue<Vector2>() * (_movementSpeed * Time.deltaTime);
            
            // Cam Oriented Movement
            Camera cam = Camera.main; // wrong
            Vector3 forward = cam.transform.forward;
            Vector3 right = cam.transform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();
            
            Vector3 inputDirection = input.x * right + input.y * forward;
            
            // Slopes
            inputDirection = Vector3.ProjectOnPlane(inputDirection, _groundCheck.GetHitNormal());
            
            _rigidbody.linearVelocity = inputDirection;
            
            // Look into Walking Direction
            if (input == Vector2.zero) return;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(inputDirection.x, 0, inputDirection.z), Vector3.up);
            _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, targetRotation, 10f * Time.fixedDeltaTime));
        }
    }
}
