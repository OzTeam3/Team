using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public enum StatType
{
    None,
    WalkSpeed,
    JumpForce
}

public class StatUpItem : ItemBase
{
    private StatUpItemData _itemData;
    private StatType _statType;

    public override void InitItem(string itemId)
    {
        _itemData = DataManager.Instance.GetData<StatUpItemData>(itemId);

        if (_itemData == null)
        {
            Debug.LogWarning($"[StatUpItem:InitItem] StatUpItemData 테이블에서 아이디를 찾을 수 없음");
            return;
        }

        bool isStatParsed = Enum.TryParse(_itemData.StatType, out _statType);

        if (!isStatParsed)
        {
            Debug.LogWarning($"[StatUpItem:InitItem] StatType 파싱 실패");
            return;
        }
    }

    public override void UseItem(PlayerController player)
    {
        if (_itemData == null)
        {
            Debug.LogWarning($"[StatUpItem:UseItem] 아이템 데이터 없음.");
            return;
        }

        player.AddStat(_statType, _itemData.Value);

        if (_itemData.Duration > 0)
        {
            ReserveDisableItem(player, _itemData.Duration).Forget();
        }
    }

    public void UnUseItem(PlayerController player)
    {
        player.AddStat(_statType, -(_itemData.Value));
    }

    private async UniTask ReserveDisableItem(PlayerController player, float duration)
    {
        CancellationToken cancelToken = player.GetCancellationTokenOnDestroy();
        TimeSpan delayTime = System.TimeSpan.FromSeconds(duration);
        bool isCancel = await UniTask.Delay(delayTime, cancellationToken: cancelToken).SuppressCancellationThrow();
        if (isCancel)
        {
            Debug.LogWarning("[StatUpItem:ReserveDisableItem] 비동기 처리 중 관련 오브젝트 파괴 됨");
            return;
        }
        UnUseItem(player);
    }
}