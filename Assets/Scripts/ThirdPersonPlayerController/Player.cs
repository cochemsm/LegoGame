using System;
using System.Collections.Generic;
using CustomStateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ThirdPersonPlayerController {
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour {
        private StateMachine _stateMachine = new StateMachine();
        private Rigidbody _rigidbody;
        private InputAction _moveInput;
        private CapsuleCollider _capsuleCollider;
        private SphereCaster _groundCheck;
        private InputAction _jumpInput;
        private InputAction _changeInput;
        private InputAction _interactInput;
        private Interactable _interactable;

        [SerializeField] private float movementSpeed = 1;
        [SerializeField] private float fallingSpeed = 1;
        [SerializeField] private float jumpForce = 1;
        [SerializeField] private string CurrentState;
        [SerializeField] private bool Grounded;
        public Vector3 Velocity;
        public Minifigure minifigure;

        [SerializeField] private List<Character> characters = new();

        public Animator Animator { get; private set; }
        public event Action<Interactable> OnInteractableChanged;
        
        private void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.freezeRotation = true;
            _rigidbody.useGravity = false;

            _capsuleCollider = GetComponent<CapsuleCollider>();
            Animator = GetComponentInChildren<Animator>();

            _groundCheck = new SphereCaster(transform.position, Vector3.down, _capsuleCollider.radius - 0.01f, _capsuleCollider.height / 2 - _capsuleCollider.radius + 0.02f, ~LayerMask.GetMask("Ignore Raycast", "Player", "RoomTrigger"));
            
            _moveInput = InputSystem.actions.FindAction("Move");
            _jumpInput = InputSystem.actions.FindAction("Jump");
            _changeInput = InputSystem.actions.FindAction("Change");
            _interactInput = InputSystem.actions.FindAction("Interact");
            
            InputSystem.actions.Enable();
            
            SetupStateMachine();
            
            ChangeCharacterByIndex(0);
        }

        private void SetupStateMachine() {
            Idle idle = new Idle(Animator);
            _stateMachine.AddState(idle);

            Walking walking = new Walking(this, _rigidbody, _moveInput, movementSpeed, _groundCheck, Animator);
            _stateMachine.AddState(walking);
            
            Falling falling = new Falling(this, _rigidbody, fallingSpeed, _moveInput, movementSpeed, Animator);
            _stateMachine.AddState(falling);
            
            Jumping jumping = new Jumping(this, transform, jumpForce, Animator);
            _stateMachine.AddState(jumping);

            Attacking attacking = new Attacking();
            _stateMachine.AddState(attacking);

            Death death = new Death();
            _stateMachine.AddState(death);

            ChangeCharacter changeCharacter = new ChangeCharacter(this, characters.Count);
            _stateMachine.AddState(changeCharacter);

            Interaction interaction = new Interaction(this, Animator);
            _stateMachine.AddState(interaction);
            
            _stateMachine.TransitionFromStateToState(idle, walking, () => _moveInput.ReadValue<Vector2>() != Vector2.zero);
            _stateMachine.TransitionFromStateToState(idle, jumping, () => _jumpInput.IsPressed());
            _stateMachine.TransitionFromStateToState(idle, changeCharacter, () => _changeInput.WasPressedThisFrame());
            _stateMachine.TransitionFromStateToState(idle, interaction, () => _interactable != null && _interactInput.WasPressedThisFrame());
            
            _stateMachine.TransitionFromStateToState(walking, idle, () => _moveInput.ReadValue<Vector2>() == Vector2.zero);
            _stateMachine.TransitionFromStateToState(walking, jumping, () => _jumpInput.IsPressed());
            _stateMachine.TransitionFromStateToState(walking, changeCharacter, () => _changeInput.WasPressedThisFrame());
            _stateMachine.TransitionFromStateToState(walking, interaction, () => _interactable != null && _interactInput.WasPressedThisFrame());
            
            _stateMachine.TransitionFromStateToState(jumping, idle, () => jumping.JumpFinished);
            
            _stateMachine.TransitionFromAnyToState(falling, () => !Grounded);
            _stateMachine.TransitionFromStateToState(falling, idle, () => Grounded);

            _stateMachine.TransitionFromStateToState(changeCharacter, idle, () => true);
            
            _stateMachine.TransitionFromStateToState(interaction, idle, () => !interaction.Interacting);
            
            _stateMachine.SetState(idle);
        }

        private void Update() {
            _groundCheck.SetOrigin(transform.position);
            _groundCheck.Cast();
            
            CurrentState = _stateMachine.CurrentState.ToString();
            Grounded = _groundCheck.HasHitSomething();
            
            _stateMachine.Update();
        }

        private void FixedUpdate() {
            _rigidbody.linearVelocity = Velocity * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out Interactable interactable)) {
                _interactable = interactable;
                OnInteractableChanged?.Invoke(interactable);
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.TryGetComponent(out Interactable interactable)) {
                _interactable = null;
            }
        }

        private void OnDrawGizmos() {
            _groundCheck?.GizmosDebug();
        }

        public void ChangeCharacterByIndex(int index) {
            var parts = GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < parts.Length; i++) {
                parts[i].material = characters[index].Material[i];
            }
            characters[index].HeadGear.SetActive(true);
            int previous = index - 1;
            if (previous == -1) previous = characters.Count - 1;
            characters[previous].HeadGear.SetActive(false);
            index += 1;
            if (index > characters.Count) index = 1;
            minifigure = (Minifigure) index;
        }
    }
}

[Serializable]
public struct Character {
    public List<Material> Material;
    public GameObject HeadGear;
}

public enum Minifigure {
    All,
    Guard,
    Janitor,
    Curator
}