using CustomStateMachine;
using UnityEngine;

namespace ThirdPersonPlayerController {
    public class Jumping : State {
        private ThirdPersonPlayerController _player;
        private float _jumpForce;
        private Animator _animator;
        private Transform _transform;
        
        public bool JumpFinished;
        private Vector3 _startingPosition;

        public Jumping(ThirdPersonPlayerController player, Transform transform, float jumpForce, Animator animator) {
            _player = player;
            _transform = transform;
            _jumpForce = jumpForce;
            _animator = animator;
        }
        
        public override void Enter() {
            _animator.Play("JumpStart");
            _player.Velocity = new Vector3(_player.Velocity.x, _jumpForce, _player.Velocity.z);
            JumpFinished = true;
        }

        public override void Exit() {
            JumpFinished = false;
        }
    }
}
