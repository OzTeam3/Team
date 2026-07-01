using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PendulumTrap : TrapBase
{
    [SerializeField] private float _timeOffset = 0f;

    private Rigidbody _trapRigidbody;

    private Quaternion _startRotation;
   
    private float _swingSpeed;
    private float _maxAngle;
    private float _knockbackForce;

    private void Awake()
    {
        if (!TryGetComponent(out _trapRigidbody))
        {
            Debug.LogError("[PendulumTrap] Rigidbody 컴포넌트가 없습니다.");
            return;
        }

        _trapRigidbody.centerOfMass = Vector3.zero;
        _startRotation = _trapRigidbody.rotation;
    }

    public override void Init(string trapId, TrapData data)
    {
        _swingSpeed = data.SpinSpeed;
        _knockbackForce = data.KnockbackForce;
        _maxAngle = data.MaxAngle;
    }

    private void FixedUpdate()
    {
        float currentAngle = Mathf.Sin((Time.fixedTime + _timeOffset) * _swingSpeed) * _maxAngle;

        Quaternion targetRotation = _startRotation * Quaternion.AngleAxis(currentAngle, Vector3.forward);

        _trapRigidbody.MoveRotation(targetRotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        ApplyKnockback(collision, _trapRigidbody, _knockbackForce);
    }
}
