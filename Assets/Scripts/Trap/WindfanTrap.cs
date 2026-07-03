using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FanTrap : MonoBehaviour
{
    //헤더 다 영엉로 수정
    [Header("바람 물리 설정")]
    [SerializeField] private ForceMode _windForceMode = ForceMode.Acceleration; //시리얼라이즈 필드 제거
    [SerializeField] private Transform _windPivot; //이게 뭔지 확인해보기

    [Header("거리 비례 감소 설정")]
    [SerializeField] private bool _useDistanceFalloff = true;

    [Header("날개 설정")]
    [SerializeField] private Transform _wing;

    //데이터 드리븐
    private float _spinSpeed = 720;
    private float _maxWindStrength = 50;
    private float _maxDistance = 10;


    private void Awake()
    {
        //엥?
        if (_windPivot == null)
        {
            _windPivot = transform;
        }
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

       // _spinSpeed = data.ActionValue;
       /// _maxWindStrength = data.Value1;
      //  _maxDistance = data.Value2;
    }

    private void Update()
    {
        if (_wing != null)
        {
            _wing.Rotate(0f, 0f, _spinSpeed * Time.deltaTime);
        }
    }
    
    //변수명 고민하기
    //메서드화 고민하기
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (!other.TryGetComponent(out Rigidbody rb))
        {
            return;
        }

        float currentStrength = _maxWindStrength;

        Vector3 windDirection = _windPivot.forward;

        if (_useDistanceFalloff)
        {
            float distance = Vector3.Distance(_windPivot.position, rb.position);

            float falloffRatio = Mathf.Clamp01(1f - (distance / _maxDistance));
            currentStrength *= falloffRatio;
        }

        Vector3 appliedForce = windDirection * currentStrength;
        rb.AddForce(appliedForce, _windForceMode);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = _windPivot != null ? _windPivot.position : transform.position;
        Vector3 direction = _windPivot != null ? _windPivot.forward : transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + direction * _maxDistance);

        Gizmos.DrawWireSphere(origin + direction * _maxDistance, 0.5f);
    }
}