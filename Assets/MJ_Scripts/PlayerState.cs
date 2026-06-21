using UnityEngine;

public enum EntityState
{
    None = 0,
    Idle,
    Walk,
    Jump
}

public class PlayerState
{
    public PlayerState(Animator animator)
    {
        _animator = animator;
    }

    private Animator _animator;

    private EntityState _currentState;

    public void SetState(EntityState newState)
    {
        if (newState == EntityState.Idle && _currentState == EntityState.Idle)
        {
            return;
        }

        _currentState = newState;

        ResetAllState();

        switch (_currentState)
        {
            case EntityState.Idle:
                break;
            case EntityState.Walk:
                _animator.SetBool("Walk", true);
                break;
            case EntityState.Jump:
                _animator.SetBool("Jump", true);
                break;
        }
    }

    private void ResetAllState()
    {
        _animator.SetBool("Walk", false);
        _animator.SetBool("Jump", false);
    }
}
