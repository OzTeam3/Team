using UnityEngine;

public class SpinTrap : MonoBehaviour
{
    [SerializeField] private float _spinSpeed = 400f;
    [SerializeField] private float _knockbackForce = 12f;

    [SerializeField] private Rigidbody _trapRigidbody;

    private void Awake()
    {
        if (_trapRigidbody == null)
        {
            _trapRigidbody = GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {
        Quaternion deltaSpin = Quaternion.Euler(Vector3.up * _spinSpeed * Time.fixedDeltaTime);

        _trapRigidbody.MoveRotation(_trapRigidbody.rotation * deltaSpin);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out KnockbackReceiver receiver))
        {
            Vector3 hitPoint = collision.contacts[0].point;

            Vector3 hitDirection = _trapRigidbody.GetPointVelocity(hitPoint);

            if (hitDirection.sqrMagnitude < 0.1f)
            {
                hitDirection = collision.transform.position - transform.position;
            }

            hitDirection.y = 0;
            hitDirection = hitDirection.normalized;
            hitDirection.y = 0.5f;

            Vector3 finalPush = hitDirection.normalized;

            receiver.ApplyKnockback(finalPush, _knockbackForce);
        }
    }
}
