using UnityEngine;

public class KnockbackReceiver : MonoBehaviour
{
    [SerializeField] private Rigidbody _entityRigidbody;

    private void Awake()
    {
        if( _entityRigidbody == null)
        {
            _entityRigidbody = GetComponent<Rigidbody>();
        }
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (_entityRigidbody == null)
        {
            return;
        }

        _entityRigidbody.linearVelocity = Vector3.zero;

        _entityRigidbody.AddForce(direction *  force, ForceMode.Impulse);
    }
}
