using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private Vector3 _bounceDirection = new Vector3(1f, 1f, 0f);
    [SerializeField] private float _bounceForce = 15.0f;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerFeet"))
        {
            var receiver = other.GetComponentInParent<KnockbackReceiver>();

            if (receiver != null) 
            {
                if (_animator != null)
                {
                    _animator.SetTrigger("DoBounce");
                }

                Vector3 finalDirection = transform.TransformDirection(_bounceDirection).normalized;

                receiver.ApplyKnockback(finalDirection, _bounceForce);
            }
        }
    }
}
