using CustomStateMachine;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ThirdPersonPlayerController {
    public class Falling : State {
        private Player _player;
        private Rigidbody _rigidbody;
        private float _fallingSpeed;
        private InputAction _moveAction;
        private float _movementSpeed;
        private Animator _animator;
        private CinemachineCamera _camera;

        public Falling(Player player, Rigidbody rigidbody, float fallingSpeed, InputAction moveAction, float movementSpeed,  Animator animator) {
            _player = player;
            _rigidbody = rigidbody;
            _fallingSpeed = fallingSpeed;
            _moveAction = moveAction;
            _movementSpeed = movementSpeed;
            _animator = animator;
            RoomManager.OnCameraChange += camera => _camera = camera;
        }

        public override void Enter() {
            _animator.Play("InAir");
        }

        public override void Update() {
            Vector2 input = _moveAction.ReadValue<Vector2>() * _movementSpeed;
            
            // Cam Oriented Movement
            Vector3 forward = _camera.transform.forward;
            Vector3 right = _camera.transform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();
            
            Vector3 inputDirection = input.x * right + input.y * forward;
            inputDirection += new Vector3(0, _player.Velocity.y, 0);
            inputDirection += Vector3.down *  _fallingSpeed;
            
            _player.Velocity = inputDirection;
            
            // Look into Walking Direction
            if (input == Vector2.zero) return;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(inputDirection.x, 0, inputDirection.z), Vector3.up);
            _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, targetRotation, 10f * Time.fixedDeltaTime));
        }

        public override void Exit() {
            _player.Velocity = Vector3.zero;
            _animator.Play("JumpEnd");
        }
    }
}
