using UnityEngine;

public class JumpPad : TrapBase
{
    private float _bounceForce;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();

        if ( _animator == null)
        {
            Debug.LogWarning("[JumpPad] Animator를 찾지 못했습니다.");
            return;
        }
    }

    public override void Init(string trapId, TrapData data)
    {
        _bounceForce = data.BounceForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        ApplyBounce(other.attachedRigidbody);
    }

    private void ApplyBounce(Rigidbody target)
    {
        if (target == null)
        {
            return;
        }

        _animator.SetTrigger("DoBounce");

        Vector3 currentVelocity = target.linearVelocity;

        if (currentVelocity.y < 0f)
        {
            currentVelocity.y = 0f;
        }

        target.linearVelocity = currentVelocity;
        target.AddForce(transform.up * _bounceForce, ForceMode.VelocityChange);
    }
}
