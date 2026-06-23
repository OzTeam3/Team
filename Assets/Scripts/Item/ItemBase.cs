using UnityEngine;

public enum ItemType
{
    None,
    AddBuff    
}
public abstract class ItemBase
{
    protected string _itemName;
    public abstract void OnAcquire(PlayerView aquirer);
    public abstract void OnUse();
}