using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BlinkGroup
{
    public List<GameObject> _blinkPlatform = new List<GameObject>();
}

public class BlinkFloor : TrapBase
{
    [Header("Blink Group Settings")]
    [SerializeField] private List<BlinkGroup> _blinkGroups = new List<BlinkGroup>();

   
    private float _delay;
    private float _switchTime;

    private Coroutine _blinkCoroutine;

    private WaitForSeconds _delayWait;
    private WaitForSeconds _switchWait;


    public override void Init(string trapId, TrapData data)
    {
        _delay = data.WaitingTime;
        _switchTime = data.RespawnTime;

        if (_blinkGroups.Count == 0)
        {
            Debug.LogError("[BlinkFloor] 지정된 그룹이 없습니다.");
            return;
        }

        _delayWait = new WaitForSeconds(_delay);
        _switchWait = new WaitForSeconds(_switchTime);
    }

    private void OnEnable()
    {
        StartBlinkLoop();
    }

    private void OnDisable()
    {
        StopBlinkLoop();
    }

    private void StartBlinkLoop()
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
        }
        _blinkCoroutine = StartCoroutine(BlinkLoop());
    }

    private void StopBlinkLoop()
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }
    }

    private IEnumerator BlinkLoop()
    {
        for (int i = 0; i < _blinkGroups.Count; i++)
        {
            SetGroupActive(_blinkGroups[i], true);
        }

        yield return _delayWait;

        while (true)
        {
            for (int i = 0; i < _blinkGroups.Count; i++)
            {
                SetGroupActive(_blinkGroups[i], false);

                yield return _switchWait;

                SetGroupActive(_blinkGroups[i], true);
            }
        }
    }

    private void SetGroupActive(BlinkGroup blinkGroup, bool isEnabled)
    {
        if (blinkGroup._blinkPlatform == null)
        {
            return;
        }

        foreach (GameObject platform in blinkGroup._blinkPlatform)
        {
            if (platform == null)
            {
                continue;
            }
            platform.SetActive(isEnabled);
        }
    }
}
