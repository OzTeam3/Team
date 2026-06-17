using UnityEngine;
using System.Collections;

public class BlinkFloor : MonoBehaviour
{
    [Header("타이밍 설정")]
    [SerializeField] private float _delay = 2.0f; // 게임 시작 후 대기 시간
    [SerializeField] private float _switchTime = 5.0f;

    [Header("교차할 그룹")]
    [SerializeField] private GameObject _groupA;
    [SerializeField] private GameObject _groupB;

    private MeshRenderer _mesh;
    private Collider _collider;
    
    private void Awake()
    {
        _mesh = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        if (_groupA == null || _groupB == null)
        {
            return;
        }

        StartCoroutine(BlinkLoop());
    }

    private IEnumerator BlinkLoop()
    {
        // 처음에 설정한 시간만큼 대기
        yield return new WaitForSeconds(_delay);

        while (true)
        {
            // [1단계] Group A 활성화, Group B 비활성화
            _groupA.SetActive(true);
            _groupB.SetActive(false);

            // 지정된 시간만큼 대기
            yield return new WaitForSeconds(_switchTime);

            // [2단계] Group A 비활성화, Group B 활성화
            _groupA.SetActive(false);
            _groupB.SetActive(true);

            // 다시 지정된 시간만큼 대기
            yield return new WaitForSeconds(_switchTime);
        }
    }

    //private IEnumerator BlinkLoop()
    //{
    //    yield return new WaitForSeconds(_delay);

    //    while (true)
    //    {
    //        TurnOn();
    //        yield return new WaitForSeconds(_onTime);

    //        TurnOff();

    //        yield return new WaitForSeconds(_offTime);
    //    }
    //}

    //private void TurnOn()
    //{
    //    if (_mesh != null) _mesh.enabled = true;
    //    if (_collider != null) _collider.enabled = true;
    //}

    //private void TurnOff()
    //{
    //    if (_mesh != null) _mesh.enabled = false;
    //    if (_collider != null) _collider.enabled = false;
    //}
}
