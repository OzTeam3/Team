using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WindFanTrap : TrapBase
{
    [Header("Wing Settings")]
    [SerializeField] private Transform _wing;

    private ForceMode _windForceMode = ForceMode.Acceleration;

    private float _spinSpeed;
    private float _maxWindStrength;
    private float _maxDistance;
    private bool _useDistanceFalloff = true;
    private Rigidbody _targetRigidbody;


    public override void Init(string trapId, TrapData data)
    {
        _spinSpeed = data.SpinSpeed;
        _maxWindStrength = data.MaxWindStrength;
        _maxDistance = data.MaxDistance;
    }

    private void Update()
    {
        if (_wing != null)
        {
            _wing.Rotate(0f, 0f, _spinSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (_targetRigidbody == null)
        {
            return;
        }
        ApplyWindForce(_targetRigidbody);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (!other.TryGetComponent(out Rigidbody targetRigidbody))
        {
            return;
        }
        _targetRigidbody = targetRigidbody;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Rigidbody targetRigidbody))
        {
            return;
        }

        if (_targetRigidbody == targetRigidbody)
        {
            _targetRigidbody = null;
        }
    }

    private void ApplyWindForce(Rigidbody targetRigidbody)
    {
        float currentStrength = _maxWindStrength;
        Vector3 windDirection = transform.forward;

        if (_useDistanceFalloff)
        {
            float distance = Vector3.Distance(transform.position, targetRigidbody.position);
            float falloffRatio = Mathf.Clamp01(1f - (distance / _maxDistance));
            currentStrength *= falloffRatio;
        }

        Vector3 appliedForce = windDirection * currentStrength;
        targetRigidbody.AddForce(appliedForce, _windForceMode);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + direction * _maxDistance);
        Gizmos.DrawWireSphere(origin + direction * _maxDistance, 0.5f);
    }
}