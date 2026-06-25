using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BlinkGroup
{
    public List<GameObject> objects = new List<GameObject>();
}

public class BlinkFloor : MonoBehaviour
{
    [Header("교차할 그룹")]
    [SerializeField] private List<BlinkGroup> _groups = new List<BlinkGroup>();

    private float _delay;
    private float _switchTime;
    private Coroutine _blinkCoroutine;
    private bool _hasStarted = false;

    private WaitForSeconds _delayWait;
    private WaitForSeconds _switchWait;

    public void Init(string trapId)
    {
        TrapData data = DataManager.Instance.GetData<TrapData>(trapId);
        if (data == null)
        {
            Debug.Log($"{trapId}에 해당하는 데이터가 없습니다.");
            return;
        }

        _delay = data.Value;
        _switchTime = data.ActionSpeed;

        if (_groups.Count == 0)
        {
            Debug.LogWarning("[BlinkFloor] 지정된 그룹이 없습니다.");
            return;
        }

        _delayWait = new WaitForSeconds(_delay);
        _switchWait = new WaitForSeconds(_switchTime);

        _hasStarted = true;
        StartBlinkLoop();
    }

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

    private void SetGroupActive(BlinkGroup group, bool isActive)
    {
        if (group == null || group.objects == null)
        {
            return;
        }

        foreach (GameObject obj in group.objects)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }
        }
    }
}
