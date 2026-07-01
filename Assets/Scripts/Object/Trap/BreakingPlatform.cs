using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public class BreakingPlatform : TrapBase
{
    [Header("Platform Settings")]
    [SerializeField] private GameObject _trapPlatform;

    private int _waitingTime;
    private int _respawnTime;
    private Animator _animator;
    private bool _isTriggered;

    private CancellationTokenSource _cancellationTokenSource;

    private void Awake()
    {
        if (_trapPlatform == null)
        {
            Debug.LogError("[FallingPlatform] _trapFloor가 지정되지 않았습니다.");
            return;
        }

        if (_animator == null)
        {
            _animator = _trapPlatform.GetComponentInChildren<Animator>();
            
            if (_animator == null)
            {
                Debug.LogWarning("[FallingPlatform] Animator를 찾지 못했습니다.");
                return;
            }
        }
    }

    public override void Init(string trapId, TrapData data)
    {
        _waitingTime = data.WaitingTime * 1000;
        _respawnTime = data.RespawnTime * 1000;
    }

    private void OnEnable()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        ResetPlatform();
    }

    private void OnDisable()
    {
        if (_cancellationTokenSource == null)
        {
            return;
        }

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null)
        {
            return;
        }
        if (_isTriggered)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            BreakingAsync(_cancellationTokenSource.Token).Forget();
        }
    }


    private async UniTask BreakingAsync(CancellationToken cancellationToken)
    {
        _isTriggered = true;

        if (_animator != null)
        {
            _animator.SetTrigger("IsShake");
        }

        bool isCanceled = await UniTask.Delay(_waitingTime, cancellationToken: cancellationToken).SuppressCancellationThrow();

        if (isCanceled)
        {
            _isTriggered = false;
            return;
        }

        _trapPlatform.SetActive(false);

        isCanceled = await UniTask.Delay(_respawnTime, cancellationToken: cancellationToken).SuppressCancellationThrow();

        if (isCanceled)
        {
            _isTriggered = false;
            return;
        }

        _trapPlatform.SetActive(true);
        _isTriggered = false;
    }

    private void ResetPlatform()
    {
        _isTriggered = false;

        if (_trapPlatform != null && !_trapPlatform.activeSelf)
        {
            _trapPlatform.SetActive(true);
        }

        if (_animator != null)
        {
            _animator.Rebind();
            _animator.Update(0f);
        }
    }
}
