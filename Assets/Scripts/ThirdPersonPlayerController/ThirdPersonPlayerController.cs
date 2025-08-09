using System;
using CustomStateMachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace ThirdPersonPlayerController {
    [RequireComponent(typeof(Rigidbody))]
    public class ThirdPersonPlayerController : MonoBehaviour {
        private StateMachine _stateMachine = new StateMachine();
        private Rigidbody _rigidbody;
        private InputAction _moveInput;

        [SerializeField] private float movementSpeed;

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start() {
            _moveInput = InputSystem.actions.FindAction("Move");
            
            InputSystem.actions.Enable();
            
            SetupStateMachine();
        }

        private void SetupStateMachine() {
            Idle idle = new Idle();
            _stateMachine.AddState(idle);

            Walking walking = new Walking(_rigidbody, _moveInput, movementSpeed);
            _stateMachine.AddState(walking);

            Jumping jumping = new Jumping();
            _stateMachine.AddState(jumping);

            Falling falling = new Falling();
            _stateMachine.AddState(falling);

            Attacking attacking = new Attacking();
            _stateMachine.AddState(attacking);

            Death death = new Death();
            _stateMachine.AddState(death);
            
            // Idle -> Walking (MoveInput != Vector2.Zero)
            _stateMachine.TransitionFromStateToState(idle, walking, () => _moveInput.ReadValue<Vector2>() != Vector2.zero);
            // Idle -> Jumping (JumpInput == true)
            // Idle -> Falling (IsGrounded == false)
            // Idle -> Attacking (AttackInput == true)
            
            // walking -> Idle (MoveInput == Vector2.Zero)
            _stateMachine.TransitionFromStateToState(walking, idle, () => _moveInput.ReadValue<Vector2>() == Vector2.zero);
            // walking -> Jumping (JumpInput == true)
            // walking -> Falling (IsGrounded == false)
            // walking -> attacking (AttackInput == true)
            
            // Jumping -> Falling (Automatic)
            // Falling -> Idle (IsGrounded == true)
            // Attacking -> Idle (Automatic)
            // Death -> Idle (Automatic)
            
            // Any -> Death (HP == 0)
            
            _stateMachine.SetState(idle);
        }
    }
}
