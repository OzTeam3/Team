using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class JumpItem : ItemBase
{
    private PlayerView _owner;
    private float _increasingForce = 10f;
    private float _duration = 10f;

    public override void OnAcquire(PlayerView aquirer)
    {
        _owner = aquirer;
    }

    public override void OnUse()
    {
        ApplyBuff();
    }
    private void IncreaseForce()
    {
        _owner.AddJumpForce(_increasingForce);
    }
    private void DecreaseForce()
    {
        _owner.AddJumpForce(-_increasingForce);
    }
    private void ApplyBuff()
    {
        IncreaseForce();
        CancellationToken cancelToken = _owner.GetCancellationTokenOnDestroy();
        EraseJumpBuffAsync(_duration, cancelToken).Forget();
    }
    private async UniTask EraseJumpBuffAsync(float duration, CancellationToken cancelToken)
    {
        int tick = (int)(duration * 1000);
        await UniTask.Delay(tick, cancellationToken: cancelToken);
        DecreaseForce();
    }
}