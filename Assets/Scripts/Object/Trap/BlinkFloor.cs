using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//굳이 두개로 나눠어야 할까?
[System.Serializable]
public class BlinkGroup
{
    //변수명 수정
    public List<GameObject> objects = new List<GameObject>();
}

//프리팹을 전부 꺼놓은 상태로 유지
//초기화 하고 생성된 프리팹을 SetActive = true;

//체크로그 다뺴주세요
public class BlinkFloor : MonoBehaviour
{
    [Header("교차할 그룹")]
    //변수명 수정
    [SerializeField] private List<BlinkGroup> _groups = new List<BlinkGroup>();

   
    private float _delay;
    private float _switchTime;
    private Coroutine _blinkCoroutine;
    private bool _hasStarted;

    //코루틴 여기로
    private WaitForSeconds _delayWait;
    private WaitForSeconds _switchWait;

    public void Init(string trapId)
    {
        TrapData data = DataManager.Instance.GetData<TrapData>(trapId);
        if (data == null)
        {
            //에러로 바꿔야되여
            Debug.Log($"{trapId}에 해당하는 데이터가 없습니다.");
            return;
        }

        _delay = data.Value1;
        _switchTime = data.Value2;

        if (_groups.Count == 0)
        {
            //에러로 수정
            Debug.LogWarning("[BlinkFloor] 지정된 그룹이 없습니다.");
            return;
        }

        _delayWait = new WaitForSeconds(_delay);
        _switchWait = new WaitForSeconds(_switchTime);

        _hasStarted = true;
        StartBlinkLoop();
    }

    //둘중 택1 하세요
    private void OnEnable()
    {
        if (_hasStarted)
        {
            StartBlinkLoop();
        }
    }

    private void OnDisable()
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }
    }

    private void StartBlinkLoop()
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
        }
        _blinkCoroutine = StartCoroutine(BlinkLoop());
    }

    private IEnumerator BlinkLoop()
    {
        for (int i = 0; i < _groups.Count; i++)
        {
            SetGroupActive(_groups[i], true);
        }

        yield return _delayWait;

        while (true)
        {
            for (int i = 0; i < _groups.Count; i++)
            {
                SetGroupActive(_groups[i], false);

                yield return _switchWait;

                SetGroupActive(_groups[i], true);
            }
        }
    }

    //isActive 변수명 수정
    private void SetGroupActive(BlinkGroup group, bool isActive)
    {
        //분활
        if (group == null || group.objects == null)
        {
            return;
        }

        //obj 변수명 수정
        //얼리 컨틴뉴 수정
        foreach (GameObject obj in group.objects)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }
        }
    }
}
