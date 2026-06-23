using Cysharp.Threading.Tasks;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("타이밍 설정")]
    [SerializeField] private float _waitingTime = 1.0f; 
    [SerializeField] private float _respawnTime = 5.0f;

    private Animator _animator;

    private bool _isTriggered = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isTriggered && other.CompareTag("PlayerFeet"))
        {
            FallingAsync().Forget();
        }
    }


    private async UniTaskVoid FallingAsync()
    {
        _isTriggered = true;

        if (_animator != null)
        {
            _animator.SetTrigger("IsShake");
        }

        await UniTask.Delay(System.TimeSpan.FromSeconds(_waitingTime), cancellationToken: this.GetCancellationTokenOnDestroy());

        gameObject.SetActive(false);

        await UniTask.Delay(System.TimeSpan.FromSeconds(_respawnTime), cancellationToken: this.GetCancellationTokenOnDestroy());

        gameObject.SetActive(true);

        _isTriggered = false;
    }
}
