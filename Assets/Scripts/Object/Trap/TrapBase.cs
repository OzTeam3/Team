using UnityEngine;

public abstract class TrapBase : MonoBehaviour
{
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