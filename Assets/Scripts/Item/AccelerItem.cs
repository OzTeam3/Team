using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;
// Value와 enum을 따라서 Player에서 값을 조정하는 방식은 어떤가.

public class AccelerItem : ItemBase
{
    private float _increasingSpeed = 10f;
    private float _duration = 10f;

    private PlayerView _owner;

    public override void OnAcquire(PlayerView aquirer)
    {
        _owner = aquirer;
    }
    public override void OnUse()
    {
        ApplyBuff();
    }
    private void IncreaseMoveSpeed()
    {
        _owner.AddMoveSpeed(_increasingSpeed);
    }
    private void DecreaseMoveSpeed()
    {
        _owner.AddMoveSpeed(-_increasingSpeed);
    }
    private void ApplyBuff()
    {
        IncreaseMoveSpeed();
        CancellationToken cancelToken = _owner.GetCancellationTokenOnDestroy();
        // 아예 그냥. 버프/디버프 같은 효과를 따로 분리 시키는 건 어떨까.
        // 아이템은 A 버프를 부여. 하고 끝.
        EraseSpeedBuffAsync(_duration, cancelToken).Forget();
    }
    private async UniTask EraseSpeedBuffAsync(float duration, CancellationToken cancelToken)
    {
        int tick = (int)(duration * 1000);
        await UniTask.Delay(tick, cancellationToken: cancelToken);
        DecreaseMoveSpeed();
    }
}