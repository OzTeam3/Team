using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class PlayerView : MonoBehaviour
{
    [Header("플레이어 설정")]
    [SerializeField] private float _movespeed = 5f;
    [SerializeField] private float _rotationSpeed = 5f;

    [Header("점프 및 물리")]
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private bool _isGrounded;

    [Header("컴포넌트")]
    [SerializeField] private Rigidbody _playerRigidbody;    //이런식으로 참조시킬지 / 어웨이크에서 트라이겟컴포넌트로 널일때 할당해줄지
    [SerializeField] private GroundDetector GroundDetector;

    //인풋매니저 넘길예정
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
        Move();
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
        if (_isGrounded)
        {
            _playerState.SetState((_playerInput.x == 0 && _playerInput.y == 0) ? EntityState.Idle : EntityState.Walk);

        }


        if (_playerInput != Vector2.zero)
        {

            Vector3 targetDir = new Vector3(_playerInput.x, 0f, _playerInput.y).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);

            _playerRigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime));
            _playerRigidbody.MovePosition(transform.position + targetDir * _movespeed * Time.fixedDeltaTime);

         
        }

        //Vector3 move = (transform.right * _playerInput.x) + (transform.forward * _playerInput.y);
        //transform.position += ((move * _movespeed) * Time.deltaTime);
    }

    private void OnGroundTriggered(bool isGrounded)
    {
        _isGrounded = isGrounded;
    }

}
