using UnityEngine;

public class PendulumTrap : MonoBehaviour
{
    [SerializeField] private float _swingSpeed = 5f;
    [SerializeField] private float _maxAngle = 60f;
    [SerializeField] private float _timeOffset = 0f;
    [SerializeField] private float _knockbackForce = 12f;

    private Rigidbody _trapRigidbody;
    private Quaternion _startRotation;

    private void Awake()
    {
        if( _trapRigidbody == null) 
        {
            _trapRigidbody = GetComponent<Rigidbody>(); 
        }

        _trapRigidbody.centerOfMass = Vector3.zero;

        _startRotation = _trapRigidbody.rotation;
    }

    private void FixedUpdate()
    {
        float currentAngle = Mathf.Sin((Time.fixedTime + _timeOffset) * _swingSpeed) * _maxAngle;

        Quaternion targetRotation = _startRotation * Quaternion.AngleAxis(currentAngle, Vector3.forward);

        _trapRigidbody.MoveRotation(targetRotation);
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