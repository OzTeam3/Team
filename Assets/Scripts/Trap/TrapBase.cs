using UnityEngine;

public abstract class TrapBase : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] protected string _trapId;

    protected virtual void Awake()
    {
        if (!string.IsNullOrWhiteSpace(_trapId))
        {
            TrapData data = DataManager.Instance.GetData<TrapData>(_trapId);
            if (data != null)
            {
                Init(_trapId, data);
            }
            else
            {
                Debug.LogError("[TrapBase] 데이터를 찾을 수 없습니다.");
            }
        }
    }

    public abstract void Init(string trapId, TrapData data);

    protected void ApplyKnockback(Collision collision, Rigidbody trapRigidbody, float knockbackForce)
    {
        

        Rigidbody target = collision.rigidbody;
        if (target == null)
        {
            return;
        }

        ContactPoint contact = collision.GetContact(0);

        Vector3 hitDirection = trapRigidbody.GetPointVelocity(contact.point);

        if (hitDirection.sqrMagnitude < 0.1f)
        {
            hitDirection = collision.transform.position - transform.position;
        }

        hitDirection.y = 0;
        hitDirection = hitDirection.normalized;
        hitDirection.y = 0.5f;

        Vector3 finalPush = hitDirection.normalized;
        
        Vector3 reflectedVelocity = Vector3.Reflect(target.linearVelocity, contact.normal);
        reflectedVelocity.y = 0f;
        target.linearVelocity = reflectedVelocity;

        target.AddForce(finalPush * knockbackForce, ForceMode.Impulse);
    }
}