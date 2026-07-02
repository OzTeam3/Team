using UnityEngine;

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

public abstract class PlayerState_Base : IPlayerState
{
    protected Animator _animator;
    protected Rigidbody _rigidbody;

    public abstract void EnterState(Player player);
    public abstract void ExitState(Player player);
    public abstract void UpdateState(Player player);
}

public class PlayerState_Idle : PlayerState_Base
{
    public override void EnterState(Player player)
    {
        if (_animator == null)
        {
            _animator = player.GetAnimator();
        }
        if (_rigidbody == null)
        {
            _rigidbody = player.GetRigidbody();
        }
        if (_animator == null || _rigidbody == null)
        {
            return;
        }

        _animator.SetBool("Idle", true);
    }

    public override void UpdateState(Player player)
    {
        if (!player.IsGrounded)
        {
            player.ChangeState(PlayerState.Jump);
            return;
        }
        if (player.IsGrab)
        {
            player.ChangeState(PlayerState.Grab);
            return;
        }

        if (player.PlayerInput != Vector2.zero)
        {
            player.ChangeState(player.IsRunning ? PlayerState.Run : PlayerState.Walk);
            return;
        }

        Idle();
    }

    public override void ExitState(Player player)
    {
        _animator.SetBool("Idle", false);
    }
    private void Idle()
    {
        _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f);
    }
}

public class PlayerState_Walk : PlayerState_Base
{
    public override void EnterState(Player player)
    {
        if (_animator == null)
        {
            _animator = player.GetAnimator();
        }
        if (_rigidbody == null)
        {
            _rigidbody = player.GetRigidbody();
        }
        if (_animator == null || _rigidbody == null)
        {
            return;
        }
        
        _animator.SetBool("Walk", true);
    }

    public override void UpdateState(Player player)
    {
        if (!player.IsGrounded)
        {
            player.ChangeState(PlayerState.Jump);
            return;
        }
        if (player.IsGrab)
        {
            player.ChangeState(PlayerState.Grab);
            return;
        }
        if (player.IsRunning)
        {
            player.ChangeState(PlayerState.Run);
            return;
        }
        if (player.PlayerInput == Vector2.zero)
        {
            player.ChangeState(PlayerState.Idle);
            return;
        }
        Move(player);
    }

    public override void ExitState(Player player)
    {
        _animator.SetBool("Walk", false);
    }

    private void Move(Player player)
    {
        Vector2 _playerInput = player.PlayerInput;

        Vector3 cameraForward = Vector3.ProjectOnPlane(player.Camera.transform.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(player.Camera.transform.right, Vector3.up).normalized;

        Vector3 targetDir = ((cameraForward * _playerInput.y) + (cameraRight * _playerInput.x)).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        _rigidbody.MoveRotation(Quaternion.Slerp(player.transform.rotation, targetRotation, player.RotationSpeed * Time.fixedDeltaTime));

        Vector3 moveOffset = player.WalkSpeed * Time.fixedDeltaTime * targetDir;
        _rigidbody.MovePosition(_rigidbody.position + moveOffset);
    }
}

public class PlayerState_Run : PlayerState_Base
{
    public override void EnterState(Player player)
    {
        if (_animator == null)
        {
            _animator = player.GetAnimator();
        }
        if (_rigidbody == null)
        {
            _rigidbody = player.GetRigidbody();
        }
        if (_animator == null || _rigidbody == null)
        {
            return;
        }

        _animator.SetBool("Run", true);
    }

    public override void UpdateState(Player player)
    {
        if (!player.IsGrounded)
        {
            player.ChangeState(PlayerState.Jump);
            return;
        }
        if (player.IsGrab)
        {
            player.ChangeState(PlayerState.Grab);
            return;
        }
        if (player.PlayerInput == Vector2.zero)
        {
            player.ChangeState(PlayerState.Idle);
            return;
        }
        if (!player.IsRunning)
        {
            player.ChangeState(PlayerState.Walk);
            return;
        }
        Move(player);
    }

    public override void ExitState(Player player)
    {
        _animator.SetBool("Run", false);
    }

    private void Move(Player player)
    {
        Vector2 _playerInput = player.PlayerInput;

        Vector3 cameraForward = Vector3.ProjectOnPlane(player.Camera.transform.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(player.Camera.transform.right, Vector3.up).normalized;

        Vector3 targetDir = ((cameraForward * _playerInput.y) + (cameraRight * _playerInput.x)).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        _rigidbody.MoveRotation(Quaternion.Slerp(player.transform.rotation, targetRotation, player.RotationSpeed * Time.fixedDeltaTime));

        Vector3 moveOffset = targetDir * player.RunSpeed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_rigidbody.position + moveOffset);
    }
}

public class PlayerState_Jump : PlayerState_Base
{
    public override void EnterState(Player player)
    {
        if (_animator == null)
        {
            _animator = player.GetAnimator();
        }
        if (_rigidbody == null)
        {
            _rigidbody = player.GetRigidbody();
        }
        if (_animator == null || _rigidbody == null)
        {
            return;
        }

        _animator.SetBool("Jump", true);
    }

    public override void UpdateState(Player player)
    {
        if (player.IsGrab)
        {
            player.ChangeState(PlayerState.Grab);
            return;
        }

        if (player.IsGrounded)
        {
            player.ChangeState(PlayerState.Idle);
        }
        
        Move(player);
    }

    public override void ExitState(Player player)
    {
        _animator.SetBool("Jump", false);
    }

    private void Move(Player player)
    {
        Vector2 _playerInput = player.PlayerInput;

        if (_playerInput == Vector2.zero)
        {
            return;
        }

        Vector3 cameraForward = Vector3.ProjectOnPlane(player.Camera.transform.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(player.Camera.transform.right, Vector3.up).normalized;

        Vector3 targetDir = ((cameraForward * _playerInput.y) + (cameraRight * _playerInput.x)).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        _rigidbody.MoveRotation(Quaternion.Slerp(player.transform.rotation, targetRotation, player.RotationSpeed * Time.fixedDeltaTime));

        Vector3 moveOffset = targetDir * player.JumpSpeed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_rigidbody.position + moveOffset);
    }
}

public class PlayerState_Grab : PlayerState_Base
{
    public override void EnterState(Player player)
    {
        if (_animator == null)
        {
            _animator = player.GetAnimator();
        }
        if (_rigidbody == null)
        {
            _rigidbody = player.GetRigidbody();
        }
        if (_animator == null || _rigidbody == null)
        {
            return;
        }

        _animator.SetBool("Grab", true);
        _rigidbody.useGravity = false;
        _rigidbody.linearVelocity = Vector3.zero;
    }

    public override void UpdateState(Player player)
    {
        if (!player.IsGrab)
        {
            player.ChangeState(PlayerState.Idle);
            return;
        }

        Climb(player);
    }

    public override void ExitState(Player player)
    {
        _animator.SetBool("Grab", false);
        _rigidbody.useGravity = true;

        player.SetGrab(false);
    }

    private void Climb(Player player)
    {
      
        if (player.transform.position.y < player.TargetClimbPosition.y - 0.05f)
        {
            Vector3 upTarget = new Vector3(player.transform.position.x, player.TargetClimbPosition.y, player.transform.position.z);

            Vector3 movePos = Vector3.MoveTowards(player.transform.position, upTarget, 3f * Time.fixedDeltaTime);
            _rigidbody.MovePosition(movePos);
        }
        else
        {
            Vector3 forwardTarget = new Vector3(player.TargetClimbPosition.x, player.transform.position.y, player.TargetClimbPosition.z);
            float dist = Vector3.Distance(player.transform.position, forwardTarget);

            if (dist > 0.15f)
            {
                Vector3 movePos = Vector3.MoveTowards(player.transform.position, forwardTarget, 3f * Time.fixedDeltaTime);
                _rigidbody.MovePosition(movePos);
            }
            else
            {
                player.ChangeState(PlayerState.Idle);
            }
        }
    }
}