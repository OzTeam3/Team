using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimationController : MonoBehaviour
{
    private Animator animator; 

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void PlayMove(bool isWalking)
    {
        if(animator != null)
        {
            animator.SetBool("isWalking", isWalking);
        }
    }

    public void PlayChase(bool isChasing)
    {
        if (animator != null)
        {
            animator.SetBool("isChasing", isChasing);
        }
    }

    public void PlayAttack(bool isAttacking)
    {
        if (animator != null)
        {
            animator.SetBool("isAttacking", isAttacking);
        }
    }
}