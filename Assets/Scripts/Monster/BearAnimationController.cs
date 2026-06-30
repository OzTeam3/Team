using UnityEngine;

public class BearAnimationController
{
    private readonly Animator _animator;

    public BearAnimationController(Animator animator)
    {
        _animator = animator;

    }

    public void PlayMove(bool isWalking)
    {
        if (_animator != null)
        {
            _animator.SetBool("isWalking", isWalking);
        }
    }

    public void PlayChase(bool isChasing)
    {
        if (_animator != null)
        {
            _animator.SetBool("isChasing", isChasing);
        }
    }

    public void PlayAttack(bool isAttacking)
    {
        if (_animator != null)
        {
            _animator.SetBool("isAttacking", isAttacking);
        }
    }
}