using CustomStateMachine;
using UnityEngine;

namespace ThirdPersonPlayerController {
    public class Interaction : State {
        private Player _player;
        private Animator _animator;
        private Interactable _interactable;
        
        public bool Interacting;
        
        public Interaction(Player player, Animator animator) {
            _player = player;
            _animator = animator;
            player.OnInteractableChanged += i => _interactable = i;
        }
        
        public override void Enter() {
            _player.Velocity = Vector3.zero;
            _animator.Play("Empty");
            Interacting = true;
            _interactable.Interact(() => Interacting = false);
        }
    }
}
