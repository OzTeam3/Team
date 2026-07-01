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
    public string ItemId { get; protected set; }
    public string MeshId { get; protected set; }
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
    public abstract void AcquireItem(PlayerView player);
    public abstract void UseItem(PlayerView player);
}
