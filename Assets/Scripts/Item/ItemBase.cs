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
    public abstract void InitItem(string itemId);
    public abstract void UseItem(Player player);
}
