using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("플레이어 설정")]
    [SerializeField] private float _movespeed = 10f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _acceleration = 30f;
    [SerializeField] private float _airAcceleration = 10f;


    [Header("점프 및 물리")]
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _groundDrag = 3f;
    [SerializeField] private float _airDrag = 0f;
    [SerializeField] private bool _isGrounded;

    [Header("컴포넌트")]
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private GroundDetector GroundDetector;

    [Header("카메라")]
    [SerializeField] private Camera _camera;

    [SerializeField] private InputActionReference _jumpAction;
    [SerializeField] private InputActionReference _moveAction;


    private Vector2 _playerInput;
    private PlayerState _playerState;

    private void Awake()
    {
        Animator animator = GetComponent<Animator>();
        _playerState = new PlayerState(animator);
    }

    private void OnEnable()
    {
        GroundDetector.GroundTriggeredEvent += OnGroundTriggered;

        _moveAction.action.Enable();
        _jumpAction.action.Enable();
        _jumpAction.action.started += OnJump;
        _moveAction.action.performed += OnMove;
    }

    private void FixedUpdate()
    {
        ApplyDrag();
        Move();
        ClampHorizontalSpeed();
    }

    private void OnDisable()
    {
        GroundDetector.GroundTriggeredEvent -= OnGroundTriggered;
        _moveAction.action.performed -= OnMove;
        _jumpAction.action.started -= OnJump;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _playerInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!_isGrounded)
        {
            return;
        }

        Vector3 currentVel = _playerRigidbody.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(currentVel.x, 0f, currentVel.z);

        _playerRigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        _playerState.SetState(EntityState.Jump);

        _isGrounded = false;
        Debug.Log(_isGrounded);
    }

    private void Move()
    {
        if (_playerState == null)
        {
            return;
        }

        // x가 앞이면 +1 뒤로가면 -1, 가만히 있으면 0, y는 오른쪽이 1,왼쪽이-1 가만히있으면 0
        if (!_isGrounded)
        {
            _playerState.SetState(EntityState.Jump);
        }
        else
        {
            _playerState.SetState((_playerInput.x == 0 && _playerInput.y == 0) ? EntityState.Idle : EntityState.Walk);

        }


        if (_playerInput != Vector2.zero)
        {
            Vector3 cameraForward = _camera.transform.forward;
            Vector3 cameraRight = _camera.transform.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 targetDir = (cameraForward * _playerInput.y + cameraRight * _playerInput.x).normalized;


            //Vector3 targetDir = new Vector3(_playerInput.x, 0f, _playerInput.y).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);

            _playerRigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime));

            //float currentAccel = _isGrounded ? _acceleration : _airAcceleration;
            //_playerRigidbody.AddForce(targetDir * currentAccel, ForceMode.Acceleration);

            Vector3 targetVelocity = targetDir * _movespeed;
            _playerRigidbody.linearVelocity = new Vector3(targetVelocity.x, _playerRigidbody.linearVelocity.y, targetVelocity.z);

        }
        else
        {
            // 입력 없을 때 수평 속도 즉시 제거
            _playerRigidbody.linearVelocity = new Vector3(0f, _playerRigidbody.linearVelocity.y, 0f);
        }
    }

    private void ApplyDrag()
    {
        _playerRigidbody.linearDamping = _isGrounded ? _groundDrag : _airDrag;
    }

    private void ClampHorizontalSpeed()
    {
        Vector3 horizontalVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);

        if (horizontalVelocity.magnitude > _movespeed)
        {
            Vector3 clamped = horizontalVelocity.normalized * _movespeed;
            _playerRigidbody.linearVelocity = new Vector3(clamped.x, _playerRigidbody.linearVelocity.y, clamped.z);
        }
    }

    private void OnGroundTriggered(bool isGrounded)
    {
        _isGrounded = isGrounded;
    }

}