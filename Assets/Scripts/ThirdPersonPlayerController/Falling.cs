using CustomStateMachine;
using UnityEngine;

namespace ThirdPersonPlayerController {
    public class Falling : State {
        private Rigidbody _rigidbody;
        private float _fallingSpeed;

        public Falling(Rigidbody rigidbody, float fallingSpeed) {
            _rigidbody = rigidbody;
            _fallingSpeed = fallingSpeed;
        }
        
        public override void Update() {
            _rigidbody.linearVelocity += Vector3.down * (Time.deltaTime * _fallingSpeed);
        }
    }
}
