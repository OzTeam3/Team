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
    [Header("타이밍 설정")]
    [SerializeField] private float _delay = 2.0f; // 게임 시작 후 대기 시간
    [SerializeField] private float _switchTime = 5.0f;

    [Header("교차할 그룹")]
    [SerializeField] private List<BlinkGroup> _groups = new List<BlinkGroup>();

    private WaitForSeconds _delayWait;
    private WaitForSeconds _switchWait;

    private void Start()
    {
        if (_groups.Count == 0)
        {
            return;
        }

        _delayWait = new WaitForSeconds(_delay);
        _switchWait = new WaitForSeconds(_switchTime);

        StartCoroutine(BlinkLoop());
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

        for (int i = 0; i < group.objects.Count; i++)
        {
            if (group.objects[i] != null)
            {
                group.objects[i].SetActive(isActive);
            }
        }
    }
}
