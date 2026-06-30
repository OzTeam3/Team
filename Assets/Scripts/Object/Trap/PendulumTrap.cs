using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PendulumTrap : MonoBehaviour
{
    [SerializeField] private float _timeOffset = 0f;

    private Rigidbody _trapRigidbody;

    private Quaternion _startRotation;
   
    //수정
    private float _swingSpeed = 4f;
    private float _maxAngle = 70f;
    private float _knockbackForce = 12f;

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

    //이하생략 알아서
    public void Init(string trapId)
    {
        TrapData data = DataManager.Instance.GetData<TrapData>(trapId);
        if (data == null)
        {
            Debug.Log($"{trapId}에 해당하는 데이터가 없습니다.");
            return;
        }

        _swingSpeed = data.ActionValue;
        _knockbackForce = data.KnockbackForce;
        _maxAngle = data.Value1;
    }

    private void FixedUpdate()
    {
        float currentAngle = Mathf.Sin((Time.fixedTime + _timeOffset) * _swingSpeed) * _maxAngle;

        Quaternion targetRotation = _startRotation * Quaternion.AngleAxis(currentAngle, Vector3.forward);

        _trapRigidbody.MoveRotation(targetRotation);
    }

    //메서드화 (선택)
    private void OnCollisionEnter(Collision collision)
    {
        //collider로 바꾸기
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        //콜리션의 리지드 바디 가져오기
        //가져와서 널체크 한번
        if (!collision.gameObject.TryGetComponent(out Rigidbody target))
        {
            return;
        }

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

        Vector3 playerVelocity = target.linearVelocity;
        Vector3 hitNormal = collision.GetContact(0).normal;

        Vector3 reflectedVelocity = Vector3.Reflect(playerVelocity, hitNormal);
        reflectedVelocity.y = 0f;

        target.linearVelocity = reflectedVelocity;
        target.AddForce(finalPush * _knockbackForce, ForceMode.Impulse);
    }
}
