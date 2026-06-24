using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public class BuffBase
{
    protected PlayerView _owner;
    protected float _duration;
    List<EffectPayload> _effectList;
    public void InitBuff(List<EffectPayload> effectList, float duration)
    {
        _duration = 10f;
        _effectList = effectList;
    }
    public void Morahaji()
    {
        TimerAsnyc().Forget();
    }
    public void ApplyBuff()
    {
        foreach (EffectPayload effect in _effectList)
        {
            EffectProcessor.ApplyEffect(effect);
        }
    }
    public void RemoveBuff()
    {
        foreach (EffectPayload effect in _effectList)
        {
            EffectProcessor.RemoveEffect(effect);
        }
    }
    private async UniTask TimerAsnyc()
    {
        CancellationToken cancelToken = _owner.GetCancellationTokenOnDestroy();
        TimeSpan delayTime = System.TimeSpan.FromSeconds(_duration);
        bool isCancel = await UniTask.Delay(delayTime, cancellationToken: cancelToken).SuppressCancellationThrow();
        if (isCancel)
        {
            Debug.LogWarning("Owner Object is Destroyed while Buff OnRunning");
            return;
        }
        RemoveBuff();
    }
}