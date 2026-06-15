using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class PlayerView : MonoBehaviour
{
    [Header("플레이어 설정")]
    [SerializeField] private float _movespeed = 5f;

    [Header("점프 및 물리")]
    [SerializeField] private float _jumpForce = 5f;

    [Header("컴포넌트")]
    [SerializeField] private Rigidbody _playerRigidbody;    //이런식으로 참조시킬지 / 어웨이크에서 트라이겟컴포넌트로 널일때 할당해줄지

    //인풋매니저 넘길예정
    [SerializeField] private InputActionReference _jumpAction;
    [SerializeField] private InputActionReference _moveAction;

    private Vector2 _playerInput;


    private void OnEnable()
    {
        _moveAction.action.Enable();
        _jumpAction.action.Enable();

        _jumpAction.action.started += OnJump; 
        _moveAction.action.performed += OnMove;
    }

    private void OnDisable()
    {
        _moveAction.action.performed -= OnMove;

    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _playerInput = context.ReadValue<Vector2>();


        Vector3 move = (transform.right * _playerInput.x) + (transform.forward * _playerInput.y);

        transform.position += ((move * _movespeed) * Time.deltaTime);

    }

    private void OnJump(InputAction.CallbackContext context)
    {
        _playerRigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }



    // 땅체크 추가
}
