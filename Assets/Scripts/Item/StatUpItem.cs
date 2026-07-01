using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public enum StatType
{
    None,
    MoveSpeed,
    JumpForce
}
public class StatUpItem : ItemBase
{
    PlayerView _owner;
    StatUpItemData _itemData;
    StatType _statType;

    public override void InitItem(string itemName)
    {
        base.InitItem(itemName);
        _itemData = DataManager.Instance.GetData<StatUpItemData>(itemName);
        if (_itemData == null)
        {
            Debug.LogWarning($"Can't Find Item {ItemId}");
            return;
        }
        bool isVariableStat = Enum.TryParse<StatType>(_itemData.StatType, out _statType);
        if (!isVariableStat)
        {
            _statType = StatType.None;
        }
    }
    public override void AcquireItem(PlayerView player)
    {
        _owner = player;
    }
    public override void UseItem(PlayerView player)
    {
        if (_itemData == null)
        {
            Debug.LogWarning($"Can't Find Item {ItemId}");
            return;
        }
        player.AddStat(_statType, _itemData.Value);

        if (_itemData.Duration > 0)
        {
            ReserveDisableItem(player, _itemData.Duration).Forget();
        }
    }
    public void UnUseItem(PlayerView player)
    {
        player.AddStat(_statType, -_itemData.Value);
    }
    private async UniTask ReserveDisableItem(PlayerView player, float duration)
    {
        CancellationToken cancelToken = _owner.GetCancellationTokenOnDestroy();
        TimeSpan delayTime = System.TimeSpan.FromSeconds(duration);
        bool isCancel = await UniTask.Delay(delayTime, cancellationToken: cancelToken).SuppressCancellationThrow();
        if (isCancel)
        {
            Debug.LogWarning("Owner Object is Destroyed while Buff OnRunning");
            return;
        }
        UnUseItem(player);
    }
}

public class StatUpItem_Light : ItemBase
{
    PlayerView _owner;

    public override void InitItem(string itemName)
    {
        base.InitItem(itemName);
    }
    public override void AcquireItem(PlayerView player)
    {
        _owner = player;
    }
    public override void UseItem(PlayerView player)
    {
        StatUpItemData itemData = DataManager.Instance.GetData<StatUpItemData>(ItemId);
        if (itemData == null)
        {
            Debug.LogWarning($"Can't Find Item {ItemId}");
            return;
        }
        bool isVariableStat = Enum.TryParse<StatType>(itemData.StatType, out StatType statType);
        if (!isVariableStat)
        {
            statType = StatType.None;
        }

        if (itemData == null)
        {
            Debug.LogWarning($"Can't Find Item {ItemId}");
            return;
        }
        player.AddStat(statType, itemData.Value);

        if (itemData.Duration > 0)
        {
            ReserveDisableItem(player, itemData.Duration).Forget();
        }
    }
    public void UnUseItem(PlayerView player)
    {
        StatUpItemData itemData = DataManager.Instance.GetData<StatUpItemData>(ItemId);
        if (itemData == null)
        {
            Debug.LogWarning($"Can't Find Item {ItemId}");
            return;
        }
        bool isVariableStat = Enum.TryParse<StatType>(itemData.StatType, out StatType statType);
        if (!isVariableStat)
        {
            statType = StatType.None;
        }
        player.AddStat(statType, -itemData.Value);
    }
    private async UniTask ReserveDisableItem(PlayerView player, float duration)
    {
        CancellationToken cancelToken = _owner.GetCancellationTokenOnDestroy();
        TimeSpan delayTime = System.TimeSpan.FromSeconds(duration);
        bool isCancel = await UniTask.Delay(delayTime, cancellationToken: cancelToken).SuppressCancellationThrow();
        if (isCancel)
        {
            Debug.LogWarning("Owner Object is Destroyed while Buff OnRunning");
            return;
        }
        UnUseItem(player);
    }
    private async UniTask ReserveAction(Action callback, float duration)
    {
        CancellationToken cancelToken = _owner.GetCancellationTokenOnDestroy();
        TimeSpan delayTime = System.TimeSpan.FromSeconds(duration);
        bool isCancel = await UniTask.Delay(delayTime, cancellationToken: cancelToken).SuppressCancellationThrow();
        if (isCancel)
        {
            Debug.LogWarning("Owner Object is Destroyed while Buff OnRunning");
            return;
        }
        callback();
    }
}