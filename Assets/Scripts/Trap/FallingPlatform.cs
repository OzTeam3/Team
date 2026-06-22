using Cysharp.Threading.Tasks;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("타이밍 설정")]
    [SerializeField] private float _waitingTime = 1.0f; 
    [SerializeField] private float _respawnTime = 5.0f;

    [SerializeField] private Animator _animator; //To DO: 무너지는 애니메이션 넣기

    private bool _isTriggered = false;


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

        //To Do 발판 애니메이션 추가 시 작동
        //if (_animator != null)
        //{
        //    _animator.SetTrigger("Shake");
        //}

        await UniTask.Delay(System.TimeSpan.FromSeconds(_waitingTime), cancellationToken: this.GetCancellationTokenOnDestroy());

        gameObject.SetActive(false);

        await UniTask.Delay(System.TimeSpan.FromSeconds(_respawnTime), cancellationToken: this.GetCancellationTokenOnDestroy());

        gameObject.SetActive(true);

        _isTriggered = false;
    }
}
