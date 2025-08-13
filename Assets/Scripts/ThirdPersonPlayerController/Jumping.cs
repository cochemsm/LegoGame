using CustomStateMachine;
using UnityEngine;

namespace ThirdPersonPlayerController {
    public class Jumping : State {
        private Rigidbody _rigidbody;
        private float _jumpForce;

        public Jumping(Rigidbody rigidbody, float jumpForce) {
            _rigidbody = rigidbody;
            _jumpForce = jumpForce;
        }
        
        public override void Enter() => _rigidbody.AddForce(Vector3.up * _jumpForce);
    }
}
