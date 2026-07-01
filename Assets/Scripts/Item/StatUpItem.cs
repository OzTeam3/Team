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
    Player _owner; //삭제해도될듯
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

        //아까처럼 수정
        bool isVariableStat = Enum.TryParse(_itemData.StatType, out _statType);

        if (!isVariableStat)
        {
            _statType = StatType.None;
        }
    }

    //없어지겟죠
    public override void AcquireItem(Player player)
    {
        _owner = player;
    }

    public override void UseItem(Player player)
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

    public void UnUseItem(Player player)
    {
        player.AddStat(_statType, -(_itemData.Value));
    }

    private async UniTask ReserveDisableItem(Player player, float duration)
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