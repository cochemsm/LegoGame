using CustomStateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ThirdPersonPlayerController {
    [RequireComponent(typeof(Rigidbody))]
    public class ThirdPersonPlayerController : MonoBehaviour {
        private StateMachine _stateMachine = new StateMachine();
        private Rigidbody _rigidbody;
        private InputAction _moveInput;
        private CapsuleCollider _capsuleCollider;
        private SphereCaster _groundCheck;

        [SerializeField] private float movementSpeed = 1;
        [SerializeField] private float fallingSpeed = 1;

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.freezeRotation = true;
            _rigidbody.useGravity = false;

            _capsuleCollider = GetComponent<CapsuleCollider>();

            _groundCheck = new SphereCaster(transform.position, Vector3.down, _capsuleCollider.radius - 0.01f, _capsuleCollider.height / 2 - _capsuleCollider.radius + 0.02f, ~LayerMask.GetMask("Ignore Raycast", "Player", "RoomTrigger"));
        }

        private void Start() {
            _moveInput = InputSystem.actions.FindAction("Move");
            
            InputSystem.actions.Enable();
            
            SetupStateMachine();
        }

        private void SetupStateMachine() {
            Idle idle = new Idle();
            _stateMachine.AddState(idle);

            Walking walking = new Walking(_rigidbody, _moveInput, movementSpeed, _groundCheck);
            _stateMachine.AddState(walking);

            Jumping jumping = new Jumping();
            _stateMachine.AddState(jumping);

            Falling falling = new Falling(_rigidbody, fallingSpeed);
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
            
            _stateMachine.TransitionFromAnyToState(falling, () => !_groundCheck.HasHitSomething());
            _stateMachine.TransitionFromStateToState(falling, idle, () => _groundCheck.HasHitSomething());
            // Any -> Death (HP == 0)
            
            _stateMachine.SetState(idle);
        }

        private void Update() {
            _groundCheck.SetOrigin(transform.position);
            _groundCheck.Cast();
            _stateMachine.Update();
        }

        private void OnDrawGizmos() {
            _groundCheck?.GizmosDebug();
        }
    }
}
