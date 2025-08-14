using CustomStateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ThirdPersonPlayerController {
    public class Walking : State {
        private ThirdPersonPlayerController _player;
        private Rigidbody _rigidbody;
        private readonly InputAction _moveAction;
        private float _movementSpeed;
        private SphereCaster _groundCheck;
        private Animator _animator;

        public Walking(ThirdPersonPlayerController player, Rigidbody rigidbody, InputAction moveAction, float movementSpeed, SphereCaster groundCheck, Animator animator) {
            _player = player;
            _rigidbody = rigidbody;
            _moveAction = moveAction;
            _movementSpeed = movementSpeed;
            _groundCheck = groundCheck;
            _animator = animator;
        }

        public override void Enter() {
            _animator.Play("Walking");
        }

        public override void Update() {
            Vector2 input = _moveAction.ReadValue<Vector2>() * _movementSpeed;
            
            // Cam Oriented Movement
            var cam = RoomManager.currentCamera;
            Vector3 forward = cam.transform.forward;
            Vector3 right = cam.transform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();
            
            Vector3 inputDirection = input.x * right + input.y * forward;
            
            // Slopes
            inputDirection = Vector3.ProjectOnPlane(inputDirection, _groundCheck.GetHitNormal());
            
            _player.Velocity = inputDirection;
            
            // Look into Walking Direction
            if (input == Vector2.zero) return;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(inputDirection.x, 0, inputDirection.z), Vector3.up);
            _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, targetRotation, 10f * Time.deltaTime));
        }
    }
}
