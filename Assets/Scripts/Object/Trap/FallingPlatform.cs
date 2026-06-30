using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

//falling<< ??
public class FallingPlatform : MonoBehaviour
{
    //헤더 영어로
    [Header("실제 발판")]
    [SerializeField] private GameObject _trapFloor; //변수명 수정

    private float _waitingTime;
    private float _respawnTime;
    private Animator _animator;
    private bool _isTriggered;

    private CancellationTokenSource _cancellationTokenSource;

    private void Awake()
    {
        if (_trapFloor == null)
        {
            Debug.LogError("[FallingPlatform] _trapFloor가 지정되지 않았습니다.");
            return;
        }

        if (_animator == null)
        {
            _animator = _trapFloor.GetComponentInChildren<Animator>();
            
            if (_animator == null)
            {
                Debug.LogWarning($"[FallingPlatform] '{_trapFloor.name}'에서 Animator를 찾지 못했습니다.");
                return;
            }
        }
    }

    //string 널체크
    public void Init(string trapId)
    {
        //TrapData 필드명 명확하게
        TrapData data = DataManager.Instance.GetData<TrapData>(trapId);
        if (data == null)
        {
            Debug.Log($"{trapId}에 해당하는 데이터가 없습니다.");
            return;
        }

        _waitingTime = data.Value1;
        _respawnTime = data.ActionValue;
    }

    private void OnEnable()
    {
        //꺼졋을 때 중단하도록 수정
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

        //메서드명 바꿔주세여
        ResetVisualState();
    }

    private void OnDisable()
    {
        //얼리리턴 널체크로 수정
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        //other이 널일수있나?
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
            FallingAsync(_cancellationTokenSource.Token).Forget();
        }
    }


    private async UniTask FallingAsync(CancellationToken cancellationToken)
    {
        try
        {
            _isTriggered = true;

            if (_animator != null)
            {
                _animator.SetTrigger("IsShake");
            }

            //초 한번 뺴주세요
            await UniTask.Delay((int)(_waitingTime * 1000), cancellationToken: cancellationToken);

            //널체크 안해된다
            if (_trapFloor != null)
            {
                _trapFloor.SetActive(false);
            }

            //초 뺴주세요
            await UniTask.Delay((int)(_respawnTime * 1000), cancellationToken: cancellationToken);

            //널체크 안해도된다
            if (_trapFloor != null)
            {
                _trapFloor.SetActive(true);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log($"[{gameObject.name}] 외부 비활성화로 인해 발판 로직이 취소됨");
        }
        finally
        {
            _isTriggered = false;
        }
    }

    private void ResetVisualState()
    {
        _isTriggered = false;

        if (_trapFloor != null && !_trapFloor.activeSelf)
        {
            _trapFloor.SetActive(true);
        }

        if (_animator != null)
        {
            _animator.Rebind();
            _animator.Update(0f);
        }
    }
}
