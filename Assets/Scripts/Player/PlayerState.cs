using UnityEngine;
using UnityEngine.InputSystem;



public enum PlayerState
{
    Idle,
    Walk,
    Run,
    Jump,
    Grab
}

public interface IPlayerState
{
    void EnterState(Player player);
    void UpdateState(Player player);
    void ExitState(Player player);
}




public class PlayerState_Idle : IPlayerState
{
    private Animator _animator;
    private Rigidbody _rigidbody; // 플레이어 에서 퍼블릭으로 열어 주면 이걸 빼고 player._rigidbody 이렇게 접근하는 방법도 있다.

    public void EnterState(Player player)
    {
        if (_animator == null)
        {
            _animator = player.GetAnimator();
            if (_animator == null)
            {
                return;
            }
        }
        if (_rigidbody == null)
        {
            _rigidbody = player.GetRigidbody();
            if (_rigidbody == null)
            {
                return;
            }
        }
        _animator.SetBool("Idle", true);
    }

    public void UpdateState(Player player)
    {
        if (!player._isGrounded)
        {
            player.ChangeState(PlayerState.Jump);
            return;
        }
        if (player._grab)
        {
            player.ChangeState(PlayerState.Grab);
            return;
        }
        if (player._playerInput != Vector2.zero)
        {
            if (player._running)
            {
                player.ChangeState(PlayerState.Run);
            }
            else
            {
                player.ChangeState(PlayerState.Walk);
            }
            return;
        }
        Idle();
    }

    public void ExitState(Player player)
    {
        _animator.SetBool("Idle", false);
    }
    private void Idle()
    {
        _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f);
    }

}

public class PlayerState_Walk : IPlayerState
{
    private Animator _animator;
    private Rigidbody _rigidbody;
    private Camera _camera;

    private float _speed = 5f;
    private float _rotationSpeed = 10f;

    public void EnterState(Player player)
    {
        if (_animator == null)
        {
            _animator = player.GetAnimator();
            if (_animator == null)
            {
                return;
            }
        }
        if (_rigidbody == null)
        {
            _rigidbody = player.GetRigidbody();
            if (_rigidbody == null)
            {
                return;
            }
        }
        if (_camera == null)
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                return;
            }
        }
        player.SetSpeed(_speed);
        _animator.SetBool("Walk", true);
    }

    public void UpdateState(Player player)
    {
        if (!player._isGrounded)
        {
            player.ChangeState(PlayerState.Jump);
            return;
        }
        if (player._grab)
        {
            player.ChangeState(PlayerState.Grab);
            return;
        }
        if (player._running)
        {
            player.ChangeState(PlayerState.Run);
            return;
        }
        if (player._playerInput == Vector2.zero)
        {
            player.ChangeState(PlayerState.Idle);
            return;
        }
        Move(player);
    }

    public void ExitState(Player player)
    {
        _animator.SetBool("Walk", false);
    }
    private void Move(Player player)
    {
        Vector2 _playerInput = player._playerInput;

        Vector3 cameraForward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(_camera.transform.right, Vector3.up).normalized;

        Vector3 targetDir = ((cameraForward * _playerInput.y) + (cameraRight * _playerInput.x)).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        _rigidbody.MoveRotation(Quaternion.Slerp(player.transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime));

        Vector3 moveOffset = targetDir * _speed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_rigidbody.position + moveOffset);
    }
}

public class PlayerState_Run : IPlayerState
{
    private Animator _animator;
    private Rigidbody _rigidbody;
    private Camera _camera;

    private float _speed = 5f;
    private float _rotationSpeed = 10f;

    public void EnterState(Player player)
    {
        if (_animator == null)
        {
            _animator = player.GetAnimator();
            if (_animator == null)
            {
                return;
            }
        }
        if (_rigidbody == null)
        {
            _rigidbody = player.GetRigidbody();
            if (_rigidbody == null)
            {
                return;
            }
        }
        if (_camera == null)
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                return;
            }
        }

        player.SetSpeed(_speed);
        _animator.SetBool("Run", true);
    }

    public void UpdateState(Player player)
    {
        if (!player._isGrounded)
        {
            player.ChangeState(PlayerState.Jump);
            return;
        }
        if (player._grab)
        {
            player.ChangeState(PlayerState.Grab);
            return;
        }
        if (player._playerInput == Vector2.zero)
        {
            player.ChangeState(PlayerState.Idle);
            return;
        }
        if (!player._running)
        {
            player.ChangeState(PlayerState.Walk);
            return;
        }
        Move(player);
    }

    public void ExitState(Player player)
    {
        _animator.SetBool("Run", false);
    }
    private void Move(Player player)
    {
        Vector2 _playerInput = player._playerInput;

        Vector3 cameraForward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(_camera.transform.right, Vector3.up).normalized;

        Vector3 targetDir = ((cameraForward * _playerInput.y) + (cameraRight * _playerInput.x)).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        _rigidbody.MoveRotation(Quaternion.Slerp(player.transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime));

        Vector3 moveOffset = targetDir * _speed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_rigidbody.position + moveOffset);
    }
}
public class PlayerState_Jump : IPlayerState
{
    private Animator _animator;
    private Rigidbody _rigidbody;
    private Camera _camera;

    private float _speed = 2.5f;
    private float _rotationSpeed = 10f;

    public void EnterState(Player player)
    {
        if (_animator == null)
        {
            _animator = player.GetAnimator();
            if (_animator == null)
            {
                return;
            }
        }
        if (_rigidbody == null)
        {
            _rigidbody = player.GetRigidbody();
            if (_rigidbody == null)
            {
                return;
            }
        }
        if (_camera == null)
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                return;
            }
        }
        player.SetSpeed(_speed);
        _animator.SetBool("Jump", true);
    }

    public void UpdateState(Player player)
    {
        if (player._grab)
        {
            player.ChangeState(PlayerState.Grab);
            return;
        }
        if (player._isGrounded)
        {
            if (player._playerInput == Vector2.zero)
            {
                player.ChangeState(PlayerState.Idle);
            }
            else
            {
                player.ChangeState(player._running ? PlayerState.Run : PlayerState.Walk);
            }
            return;
        }
        Move(player);
    }

    public void ExitState(Player player)
    {
        _animator.SetBool("Jump", false);
    }

    private void Move(Player player)
    {
        Vector2 _playerInput = player._playerInput;

        if (_playerInput == Vector2.zero)
        {
            return; 
        }

        Vector3 cameraForward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(_camera.transform.right, Vector3.up).normalized;

        Vector3 targetDir = ((cameraForward * _playerInput.y) + (cameraRight * _playerInput.x)).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        _rigidbody.MoveRotation(Quaternion.Slerp(player.transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime));

        Vector3 moveOffset = targetDir * _speed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_rigidbody.position + moveOffset);
    }
}

public class PlayerState_Grab : IPlayerState
{
    private Animator _animator;
    private Rigidbody _rigidbody;

    //private Vector3 _startPos;              //3안 일때 추가
    //private float _climbTimer;              //3안 일때 추가
    //private float _climbDuration = 0.5f;    //3안 일때 추가
    public void EnterState(Player player)
    {
        if (_animator == null)
        {
            _animator = player.GetAnimator();
            if (_animator == null)
            {
                return;
            }
        }
        if (_rigidbody == null)
        {
            _rigidbody = player.GetRigidbody();
            if (_rigidbody == null)
            {
                return;
            }
        }
        _animator.SetBool("Grab", true);
        _rigidbody.useGravity = false;
        //_rigidbody.isKinematic = true; // 2안일때 추가
        _rigidbody.linearVelocity = Vector3.zero; // 기존

        //_startPos = player.transform.position;     //3안 일때 추가
        //_climbTimer = 0f;                          //3안 일때 추가
    }

    public void UpdateState(Player player)
    {
        if (!player._grab)
        {
            player.ChangeState(player._isGrounded ? PlayerState.Idle : PlayerState.Jump);
            return;
        }

        Climb(player);
    }

    public void ExitState(Player player)
    {
        _animator.SetBool("Grab", false);
        _rigidbody.useGravity = true;
        //_rigidbody.isKinematic = false; // 2안일때 추가
    }

    private void Climb(Player player)
    {
        // 기존 코드
        //Vector3 targetPos = new Vector3(player._targetClimbPos.x, player.transform.position.y, player._targetClimbPos.z);
        //float distance = Vector3.Distance(player.transform.position, targetPos);

        //if (distance > 0.2f)
        //{
        //    Vector3 snapPos = Vector3.MoveTowards(player.transform.position, targetPos, 5f * Time.fixedDeltaTime);
        //    _rigidbody.MovePosition(snapPos);
        //}
        //else
        //{
        //    Vector3 climbDir = new Vector3(0, 1f, 0);
        //    _rigidbody.MovePosition(player.transform.position + (climbDir * 2f * Time.fixedDeltaTime));

        //    player._grab = false;
        //}

        // 1안 (엘리베이터처럼 올라감)
        if (player.transform.position.y < player._targetClimbPos.y - 0.05f)
        {
            Vector3 upTarget = new Vector3(player.transform.position.x, player._targetClimbPos.y, player.transform.position.z);

            Vector3 movePos = Vector3.MoveTowards(player.transform.position, upTarget, 3f * Time.fixedDeltaTime);
            _rigidbody.MovePosition(movePos);
        }
        else
        {
            Vector3 forwardTarget = new Vector3(player._targetClimbPos.x, player.transform.position.y, player._targetClimbPos.z);
            float dist = Vector3.Distance(player.transform.position, forwardTarget);

            if (dist > 0.15f)
            {
                Vector3 movePos = Vector3.MoveTowards(player.transform.position, forwardTarget, 3f * Time.fixedDeltaTime);
                _rigidbody.MovePosition(movePos);
            }
            else
            {
                player._grab = false;
            }
        }

        //2안 슝 하고 바로 올라가버림
        //float distance = Vector3.Distance(player.transform.position, player._targetClimbPos);

        //if (distance > 0.1f)
        //{
        //    Vector3 smoothPos = Vector3.Lerp(player.transform.position, player._targetClimbPos, 7f * Time.fixedDeltaTime);
        //    _rigidbody.MovePosition(smoothPos);
        //}
        //else
        //{
        //    player._grab = false;
        //}


        //3안 
        //_climbTimer += Time.fixedDeltaTime;

        //float tClimb = _climbTimer / _climbDuration;

        //float smoothT = Mathf.SmoothStep(0f, 1f, tClimb);

        //Vector3 newPos = Vector3.Lerp(_startPos, player._targetClimbPos, smoothT);

        //newPos.y += Mathf.Sin(smoothT * Mathf.PI) * 0.3f;

        //_rigidbody.MovePosition(newPos);

        //if (tClimb >= 1f)
        //{
        //    player._grab = false;
        //}
    }
}