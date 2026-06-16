using UnityEngine;
using System.Collections;

public class BlinkFloor : MonoBehaviour
{
    [Header("타이밍 설정")]
    [SerializeField] private float _delay = 2.0f; // 게임 시작 후 대기 시간
    [SerializeField] private float _onTime = 3.0f;
    [SerializeField] private float _offTime = 3.0f;

    private MeshRenderer _mesh;
    private Collider _collider;
    
    private void Awake()
    {
        _mesh = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        StartCoroutine(BlinkLoop());
    }

    private IEnumerator BlinkLoop()
    {
        yield return new WaitForSeconds(_delay);

        while (true)
        {
            TurnOn();
            yield return new WaitForSeconds(_onTime);

            TurnOff();

            yield return new WaitForSeconds(_offTime);
        }
    }

    private void TurnOn()
    {
        if (_mesh != null) _mesh.enabled = true;
        if (_collider != null) _collider.enabled = true;
    }

    private void TurnOff()
    {
        if (_mesh != null) _mesh.enabled = false;
        if (_collider != null) _collider.enabled = false;
    }
}
