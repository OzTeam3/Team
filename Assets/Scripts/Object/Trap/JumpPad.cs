using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private Vector3 _bounceDirection = new Vector3(1f, 1f, 0f);

    private float _bounceForce;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();

        if ( _animator == null)
        {
            //에러로
            Debug.Log("애니메이션이 없습니다.");
            return;
        }
    }

    //기타사항 똑같이
    public void Init(string trapId)
    {
        TrapData data = DataManager.Instance.GetData<TrapData>(trapId);
        if (data == null)
        {
            Debug.Log($"{trapId}에 해당하는 데이터가 없습니다.");
            return;
        }

        _bounceForce = data.KnockbackForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        //얼리리턴
        if (other.CompareTag("Player"))
        {  
            //메서드로 빼자(선택)
            Rigidbody target = other.attachedRigidbody;

         
            if (target != null)
            {
                if (_animator != null)
                {
                    _animator.SetTrigger("DoBounce");
                }

                Vector3 finalDirection = transform.TransformDirection(_bounceDirection).normalized;

                Vector3 currentVelocity = target.linearVelocity;
                currentVelocity.y = 0f;
                target.linearVelocity = currentVelocity;

                target.AddForce(finalDirection * _bounceForce, ForceMode.Impulse);
            }
            //트랜스폼의 Vector3.up값을 가져와서 그냥 Addforce
        }
    }
}
