using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class RollingTrapBase : TrapBase
{
    [Header("Rotation Settings")]
    [SerializeField] protected Vector3 _rotationAxis = Vector3.right;

    protected Rigidbody _platformRigidbody;
    protected Rigidbody _playerRigidbody;

    protected float _rotationSpeed;
    protected float _maxSurfaceAngle;

    protected virtual void Awake()
    {
        if (!TryGetComponent(out _platformRigidbody))
        {
            Debug.LogError($"[RollingTrap] Rigidbody가 없습니다.");
            return;
        }
    }

    public override void Init(string trapId, TrapData data)
    {
        _rotationSpeed = data.SpinSpeed;
        _maxSurfaceAngle = data.MaxAngle;
    }

    protected virtual void FixedUpdate()
    {
        Vector3 axis = _rotationAxis.normalized;
        Quaternion deltaRotation = Quaternion.AngleAxis(_rotationSpeed * Time.fixedDeltaTime, axis);
        _platformRigidbody.MoveRotation(_platformRigidbody.rotation * deltaRotation);

        if (_playerRigidbody == null)
        {
            return;
        }

        Vector3 pivot = transform.position;
        float angleRad = _rotationSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime;
        Vector3 offset = _playerRigidbody.position - pivot;
        Vector3 surfaceDirection = Vector3.Cross(axis, offset);
        Vector3 moveDelta = surfaceDirection * angleRad;

        _playerRigidbody.MovePosition(_playerRigidbody.position + moveDelta);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player"))
        {
            return;
        }
        if (collision.rigidbody == null)
        {
            return;
        }

        if (IsOnSurface(collision))
        {
            _playerRigidbody = collision.rigidbody;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.rigidbody == null)
        {
            return;
        }

        if (_playerRigidbody == collision.rigidbody)
        {
            _playerRigidbody = null;
        }
    }

    private bool IsOnSurface(Collision collision)
    {
        float minSurfaceDot = Mathf.Cos(_maxSurfaceAngle * Mathf.Deg2Rad);

        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) >= minSurfaceDot)
            {
                return true;
            }
        }
        return false;
    }
}