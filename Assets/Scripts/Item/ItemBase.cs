using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public abstract class ItemBase
{
    protected string _itemName;
    public virtual void InitItem(string itemName)
    {
        _itemName = itemName;
    }
    public abstract void AcquireItem(PlayerView player);
    public abstract void UseItem(PlayerView player);
}
