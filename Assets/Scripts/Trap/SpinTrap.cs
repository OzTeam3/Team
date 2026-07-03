using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class SpinTrap : MonoBehaviour
{
    [SerializeField] private Rigidbody _trapRigidbody;

    //데이터 드리븐
    private float _spinSpeed = 60f;
    private float _knockbackForce = 15f;

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


    //이하동문
    public void Init(string trapId)
    {
        TrapData data = DataManager.Instance.GetData<TrapData>(trapId);
        if (data == null)
        {
            Debug.Log($"{trapId}에 해당하는 데이터가 없습니다.");
            return;
        }

        //_spinSpeed = data.ActionValue;
       // _knockbackForce = data.KnockbackForce;

        Debug.Log($"[SpinTrap] {trapId} 초기화 완료! 속도: {_spinSpeed}");
    }

    //메서드로 뺴기(선택)
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (!collision.gameObject.TryGetComponent(out Rigidbody target))
        {
            return;
        }

        //중복로직 제거
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
