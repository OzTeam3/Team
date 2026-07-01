using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public enum ItemType
{
    None,
    StatUp
}

public abstract class ItemBase
{
    //얘도 삭제되도 된다.
    public string ItemId { get; protected set; }
    public string MeshId { get; protected set; }

    //얘도 abstract가 되도 된다.
    public virtual void InitItem(string itemId)
    {
        ItemData itemData = DataManager.Instance.GetData<ItemData>(itemId);

        if(itemData == null )
        {
            Debug.LogError($"[ItemBase] : There is no {itemId} in DataManager");
            return;
        }

        ItemId = itemData.Id;
        MeshId = itemData.MeshId;
    }

    public abstract void AcquireItem(Player player);

    public abstract void UseItem(Player player);
}
