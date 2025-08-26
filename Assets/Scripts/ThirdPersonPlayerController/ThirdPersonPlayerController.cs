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
        private Animator _animator;

        [SerializeField] private float movementSpeed = 1;
        [SerializeField] private float fallingSpeed = 1;
        [SerializeField] private float jumpForce = 1;
        [SerializeField] private string CurrentState;
        [SerializeField] private bool Grounded;
        public Vector3 Velocity;

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.freezeRotation = true;
            _rigidbody.useGravity = false;

            _capsuleCollider = GetComponent<CapsuleCollider>();

            _animator = GetComponentInChildren<Animator>();

            _groundCheck = new SphereCaster(transform.position, Vector3.down, _capsuleCollider.radius - 0.01f, _capsuleCollider.height / 2 - _capsuleCollider.radius + 0.02f, ~LayerMask.GetMask("Ignore Raycast", "Player", "RoomTrigger"));
            
            _moveInput = InputSystem.actions.FindAction("Move");
            _jumpInput = InputSystem.actions.FindAction("Jump");
            
            InputSystem.actions.Enable();
            
            SetupStateMachine();
        }

        private void SetupStateMachine() {
            Idle idle = new Idle(_animator);
            _stateMachine.AddState(idle);

            Walking walking = new Walking(this, _rigidbody, _moveInput, movementSpeed, _groundCheck, _animator);
            _stateMachine.AddState(walking);
            
            Falling falling = new Falling(this, _rigidbody, fallingSpeed, _moveInput, movementSpeed, _animator);
            _stateMachine.AddState(falling);
            
            Jumping jumping = new Jumping(this, transform, jumpForce, _animator);
            _stateMachine.AddState(jumping);

            Attacking attacking = new Attacking();
            _stateMachine.AddState(attacking);

            Death death = new Death();
            _stateMachine.AddState(death);
            
            // Idle -> Walking (MoveInput != Vector2.Zero)
            _stateMachine.TransitionFromStateToState(idle, walking, () => _moveInput.ReadValue<Vector2>() != Vector2.zero);
            // Idle -> Jumping (JumpInput == true)
            _stateMachine.TransitionFromStateToState(idle, jumping, () => _jumpInput.IsPressed());
            // Idle -> Attacking (AttackInput == true)
            
            // walking -> Idle (MoveInput == Vector2.Zero)
            _stateMachine.TransitionFromStateToState(walking, idle, () => _moveInput.ReadValue<Vector2>() == Vector2.zero);
            // walking -> Jumping (JumpInput == true)
            _stateMachine.TransitionFromStateToState(walking, jumping, () => _jumpInput.IsPressed());
            // walking -> attacking (AttackInput == true)
            
            // Jumping -> Falling (Automatic)
            // Falling -> Idle (IsGrounded == true)
            // Attacking -> Idle (Automatic)
            // Death -> Idle (Automatic)
            
            _stateMachine.TransitionFromStateToState(jumping, idle, () => jumping.JumpFinished);
            
            _stateMachine.TransitionFromAnyToState(falling, () => !_groundCheck.HasHitSomething());
            _stateMachine.TransitionFromStateToState(falling, idle, () => _groundCheck.HasHitSomething());
            // Any -> Death (HP == 0)
            
            _stateMachine.SetState(idle);
        }

        private void Update() {
            _groundCheck.SetOrigin(transform.position);
            _groundCheck.Cast();
            
            CurrentState = _stateMachine.CurrentState.ToString();
            Grounded = _groundCheck.HasHitSomething();
        }

        private void FixedUpdate() {
            _stateMachine.Update();
            
            _rigidbody.linearVelocity = Velocity * Time.deltaTime;
        }

        private void OnDrawGizmos() {
            _groundCheck?.GizmosDebug();
        }
    }
}
