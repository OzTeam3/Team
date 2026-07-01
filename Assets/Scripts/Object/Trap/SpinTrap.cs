using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class SpinTrap : TrapBase
{
    [SerializeField] private Rigidbody _trapRigidbody;

    private float _spinSpeed;
    private float _knockbackForce;

    private void Awake()
    {
        if (!TryGetComponent(out _trapRigidbody))
        {
            Debug.LogError("[SpinTrap] Rigidbody 컴포넌트가 없습니다.");
            return;
        }
    }

    private void FixedUpdate()
    {
        Quaternion deltaSpin = Quaternion.Euler(_spinSpeed * Time.fixedDeltaTime * Vector3.up);

        _trapRigidbody.MoveRotation(_trapRigidbody.rotation * deltaSpin);
    }

    public override void Init(string trapId, TrapData data)
    {
        _spinSpeed = data.SpinSpeed;
        _knockbackForce = data.KnockbackForce;
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
