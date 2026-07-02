using Cysharp.Threading.Tasks.Triggers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//유징 안쓰는거 제거
//테스트용 시리얼라이즈 필드 다 제거
//리소스 매니저 참고해서 에러로그 똑같이 {} 없어야된다 바꾸기
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
   // [Header("Component")]
   // [SerializeField] private Animator _animator;
   // [SerializeField] private Rigidbody _rigidbody;

   // //이 두개 Awake에서 받아오도록 해주세요
   // //GetComponentInChildren 로 받아와서 꼮 널체크 해주세요 에러까지 만들어달라는뜻
   // [SerializeField] private GroundDetector _groundDetector;
   //// [SerializeField] private LedgeDetector _ledgeDetector;

   // [Header("Camera")]
   // //시리얼 라이즈 뺴주세요
   // [SerializeField] private Camera _camera;

   // //인풋매니저 실패!!
   // [Header("Input Action")]
   // [SerializeField] private InputActionReference _jumpAction;
   // [SerializeField] private InputActionReference _moveAction;
   // [SerializeField] private InputActionReference _runAction;
   // [SerializeField] private InputActionReference _grabAction;

   // //풀이름 써주세요
   // [Header("Player Set")]
   // //님 왜 퍼블릭?
   // [SerializeField] public Vector3 _targetClimbPos;
   // //제거
   // [SerializeField] private float _climbUpOffset = 0.5f;

   // [Header("Jump Set")]
   // [SerializeField] private float _jumpForce = 5f;
   // //님 또 왜 퍼블릭?
   // //퍼블릭으로 만들경우 : 프로퍼티로 만들자
   // [SerializeField] public bool _isGrounded;

   // //잠깐 확인용
   // [SerializeField] private PlayerState _curStateEnum;

   // [SerializeField] private IPlayerState _curState;
   // [SerializeField] private Dictionary<PlayerState, IPlayerState> _playerStates;

   // //왜퍼릭
   // public Vector2 _playerInput;

   // //확인
   // private float _currentSpeed;
    
   // //왜 다 퍼블릭
   // public bool _running;

   // //false는 써도 되고 안써도된다
   // public bool _grab = false;
   // public bool _canGrab = false;

   // private void Awake()
   // {
   //     if (!TryGetComponent(out _animator))
   //     {
   //         Debug.LogError("애니메이터를 가져오지 못했습니다.");
   //         return;
   //     }
   //     if (!TryGetComponent(out _rigidbody))
   //     {
   //         Debug.LogError("애니메이터를 가져오지 못했습니다.");
   //         return;
   //     }

   //     //메서드로 빼주기
   //     _playerStates = new Dictionary<PlayerState, IPlayerState>
   //     {
   //         {PlayerState.Idle, new PlayerState_Idle() },
   //         {PlayerState.Walk, new PlayerState_Walk() },
   //         {PlayerState.Run, new PlayerState_Run() },
   //         {PlayerState.Jump, new PlayerState_Jump() },
   //         {PlayerState.Grab, new PlayerState_Grab() }
   //     };

   //     //Awake에서 해야될지 OnEnable에서 해야될지 고민
   //     ChangeState(PlayerState.Idle);
   // }


   // private void OnEnable()
   // {
   //     _groundDetector.GroundTriggeredEvent += OnGroundTriggered;
   //    // _ledgeDetector.LedgeTriggeredEvent += OnLedgeTirggered;

   //     //전부 인풋매니저로 간다!
   //     _moveAction.action.Enable();
   //     _runAction.action.Enable();
   //     _jumpAction.action.Enable();
   //     _grabAction.action.Enable();

   //     _moveAction.action.performed += OnMove;
   //     _runAction.action.performed += OnRun;
   //     _jumpAction.action.started += OnJump;
   //     _grabAction.action.started += OnClimb;
   // }

   // //제거
   // private void Update()
   // {
   // }

   // private void FixedUpdate()
   // {
   //     //널체크 해야되는지 고민
   //     if (_curState != null)
   //     {
   //         _curState.UpdateState(this);
   //     }
   // }

   // private void OnDisable()
   // {
   //     _groundDetector.GroundTriggeredEvent -= OnGroundTriggered;
   //    // _ledgeDetector.LedgeTriggeredEvent -= OnLedgeTirggered;

   //     //인풋매니저로 빠진다
   //     _moveAction.action.performed -= OnMove;
   //     _runAction.action.performed -= OnRun;
   //     _jumpAction.action.started -= OnJump;
   //     _grabAction.action.started -= OnClimb;

   //     _moveAction.action.Disable();
   //     _runAction.action.Disable();
   //     _jumpAction.action.Disable();
   //     _grabAction.action.Disable();

   // }

   // private void OnMove(InputAction.CallbackContext context)
   // {
   //     _playerInput = context.ReadValue<Vector2>();
   // }

   // private void OnJump(InputAction.CallbackContext context)
   // {       
   //     if (!_isGrounded)
   //     {
   //         return;
   //     }

   //     //이상적인 형태 점프 bool만 받아와서 판단은 상태패턴에서 변화
   //     //점프 상태로 바꾸기
   //     _isGrounded = false;

   //     _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
   // }

   // private void OnRun(InputAction.CallbackContext context)
   // {
   //     float runInput = context.ReadValue<float>();
   //     _running = runInput != 0;
   // }

   // private void OnClimb(InputAction.CallbackContext context)
   // {
   //     //얼리리턴
   //     if (_canGrab)
   //     {
   //         _grab = true;
   //     }

   //     //그랩 false상태 확인
   // }


   // private void OnGroundTriggered(bool isGrounded)
   // {
   //     _isGrounded = isGrounded;
   // }

   // private void OnLedgeTirggered(Collider ledgeCollider)
   // {
   //     if (ledgeCollider == null)
   //     {
   //         _canGrab = false;
   //         return;
   //     }

       
   //     //얘를 마지막에 해주세요
   //     _canGrab = true;
   //     //다 자워 주기
   //     //Vector3 top = ledgeCollider.bounds.max; // 사물 최고점

   //     //_targetClimbPos = ledgeCollider.transform.position + new Vector3(0, 1f, 0); // << 범인으로 예상 사물체의 y값보다 낮게 잡는거같음 + 기존
   //     //_targetClimbPos = new Vector3(ledgeCollider.transform.position.x, top.y + 0.1f, ledgeCollider.transform.position.z); //최고점 구해서 올라가는거


   //     // 트리거 감지 후 플레이어 y 살짝 위 + 끼임 현상 발생 실린더 형태의 구조물이 대각선으로 누워있을 경우 옆면 탐
   //     //_targetClimbPos = new Vector3(ledgeCollider.transform.position.x, transform.position.y + _climbUpOffset,ledgeCollider.transform.position.z);

   //     // 1. 발판 표면 중 플레이어(나)와 가장 가까운 지점을 찾습니다. (X, Z축 훅 빨려들어감 방지)
   //     Vector3 closestPoint = ledgeCollider.ClosestPoint(transform.position);

   //     // 2. 발판 오브젝트가 가진 콜라이더 박스의 가장 높은 Y값을 구합니다. (꼭대기 높이)
   //     float topY = ledgeCollider.bounds.max.y;

   //     // 3. X, Z는 부딪힌 표면으로 유지하고, Y값만 발판 꼭대기로 정렬합니다.
   //     _targetClimbPos = new Vector3(closestPoint.x, topY, closestPoint.z);
   // }

   // //넌 어따쓰고있니?
   // public void SetSpeed(float speed)
   // {
   //     _currentSpeed = speed;
   // }

   // public void ChangeState(PlayerState newState)
   // {
   //     if (_playerStates.ContainsKey(newState) == false)
   //     {
   //         Debug.LogWarning($"{newState}에 해당하는 상태 클래스를 찾을 수 없습니다! Awake에서 해당 상태 클래스를 미리 new 해두었는지 확인해주세요");
   //         return;
   //     }

   //     if (_curState != null)
   //     {
   //         _curState.ExitState(this);
   //     }

   //     _curState = _playerStates[newState];
   //     _curState.EnterState(this);
   //     _curStateEnum = newState;
   // }

   // //프로퍼티로 만들면 좀 편해질 지도?
   // public Animator GetAnimator()
   // {
   //     return _animator;
   // }

   // public Rigidbody GetRigidbody()
   // {
   //     return _rigidbody;
   // }

   // [SerializeField] private float _movespeed;

   // public void AddStat(StatType statType, float increaseStat)
   // {
   //     switch (statType)
   //     {
   //         case StatType.None:
   //             Debug.LogError("이상한 값");
   //             break;
   //         case StatType.MoveSpeed:
   //             _movespeed += increaseStat;
   //             break;
   //         case StatType.JumpForce:
   //             _jumpForce += increaseStat;
   //             break;
   //     }
   // }
}
