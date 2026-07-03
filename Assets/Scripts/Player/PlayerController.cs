using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private GroundDetector _groundDetector;
    [SerializeField] private LedgeDetector _ledgeDetector;

    [Header("Input Action")]
    [SerializeField] private InputActionReference _jumpAction;
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _runAction;
    [SerializeField] private InputActionReference _grabAction;

    private IPlayerState _currentState;
    private Dictionary<PlayerState, IPlayerState> _playerStates;
    
    public Camera Camera { get; private set; }
    public Vector2 PlayerInput { get; private set; }
    public Vector3 TargetClimbPosition { get; private set; }

    public float WalkSpeed { get; private set; }
    public float RunSpeed { get { return WalkSpeed * 2; } }
    public float JumpSpeed { get; private set; }
    public float RotationSpeed { get; private set; }
    public float JumpForce { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsGrab { get; private set; }
    public bool IsCanGrab { get; private set; }
    public bool IsGrounded { get; private set; }

    private void Awake()
    {
        if (!TryGetComponent(out _animator))
        {
            Debug.LogError("[Player:Awake] Animator 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        
        if (!TryGetComponent(out _rigidbody))
        {
            Debug.LogError("[Player:Awake] Rigidbody 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        Camera = Camera.main;

        if (Camera == null)
        {
            Debug.LogError("[Player:Awake] 카메라를 찾을 수 없습니다.");
            return;
        }

        _groundDetector = GetComponentInChildren<GroundDetector>();

        if (_groundDetector == null)
        {
            Debug.LogError("[Player:Awake] 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        _ledgeDetector = GetComponentInChildren<LedgeDetector>();

        if (_ledgeDetector == null)
        {
            Debug.LogError("[Player:Awake] 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        InitStateDictionary();
    }


    private void OnEnable()
    {
        ChangeState(PlayerState.Idle);

        _groundDetector.OnGroundTriggeredAction += OnGroundTriggered;
        _ledgeDetector.OnLedgeTriggeredAction += OnLedgeTriggered;

        //전부 인풋매니저로 간다!
        _moveAction.action.Enable();
        _runAction.action.Enable();
        _jumpAction.action.Enable();
        _grabAction.action.Enable();

        _moveAction.action.performed += OnMove;
        _runAction.action.performed += OnRun;
        _jumpAction.action.started += OnJump;
        _grabAction.action.started += OnClimb;
    }

    private void Update()
    {
        if (_currentState == null)
        {
            return;
        }

        _currentState.UpdateState(this);
    }

    private void FixedUpdate()
    {
        if (_currentState == null)
        {
            return;
        }

        _currentState.FixedUpdateState(this);
    }

    private void OnDisable()
    {
        _groundDetector.OnGroundTriggeredAction -= OnGroundTriggered;
        _ledgeDetector.OnLedgeTriggeredAction -= OnLedgeTriggered;

        _moveAction.action.performed -= OnMove;
        _runAction.action.performed -= OnRun;
        _jumpAction.action.started -= OnJump;
        _grabAction.action.started -= OnClimb;
        
        _moveAction.action.Disable();
        _runAction.action.Disable();
        _jumpAction.action.Disable();
        _grabAction.action.Disable();
    }
    public void ChangeState(PlayerState newState)
    {
        if (_playerStates.ContainsKey(newState) == false)
        {
            Debug.LogError("[Player:ChangeState] 플레이어 상태를 찾을 수 없습니다.");
            return;
        }

        if (_currentState != null)
        {
            _currentState.ExitState(this);
        }

        _currentState = _playerStates[newState];
        _currentState.EnterState(this);
    }

    public void InitPlayerData(CharacterData _characterData)
    {
        WalkSpeed = _characterData.WalkSpeed;
        JumpSpeed = _characterData.JumpSpeed;
        RotationSpeed = _characterData.RotationSpeed;
        JumpForce = _characterData.JumpForce;
    }

    public Animator GetAnimator()
    {
        return _animator;
    }

    public Rigidbody GetRigidbody()
    {
        return _rigidbody;
    }

    public void SetGrab(bool value)
    {
        IsGrab = value;
    }

    private void InitStateDictionary()
    {
        _playerStates = new Dictionary<PlayerState, IPlayerState>
        {
            {PlayerState.Idle, new PlayerState_Idle() },
            {PlayerState.Walk, new PlayerState_Walk() },
            {PlayerState.Run, new PlayerState_Run() },
            {PlayerState.Jump, new PlayerState_Jump() },
            {PlayerState.Grab, new PlayerState_Grab() }
        };
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        PlayerInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!IsGrounded)
        {
            return;
        }

        _rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        float runInput = context.ReadValue<float>();
        IsRunning = runInput != 0;
    }

    private void OnClimb(InputAction.CallbackContext context)
    {
        if (!IsCanGrab)
        {
            return;
        }

        IsGrab = true;
    }

    private void OnGroundTriggered(bool isGrounded)
    {
        IsGrounded = isGrounded;
    }

    private void OnLedgeTriggered(Collider ledgeCollider)
    {
        if (ledgeCollider == null)
        {
            IsCanGrab = false;
            return;
        }
        
        Vector3 handPos = transform.position + (transform.forward * 0.5f) + (Vector3.up * 1.5f);

        Vector3 grabPoint = ledgeCollider.ClosestPoint(handPos);

        float actualHeight = grabPoint.y;

        float heightDifference = actualHeight - transform.position.y;

        if (heightDifference < 1.0f || heightDifference > 2.2f)
        {
            IsCanGrab = false;
            return;
        }

        Vector3 ledgeNormal = (grabPoint - handPos).normalized;
        TargetClimbPosition = grabPoint - (ledgeNormal * 0.2f);

        TargetClimbPosition = new Vector3(TargetClimbPosition.x, actualHeight, TargetClimbPosition.z);

        IsCanGrab = true;
    }
}