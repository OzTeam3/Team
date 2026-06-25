using UnityEngine;

public enum ItemType
{
    None,
    AddBuff
}


public abstract class ItemBase
{
    protected string _itemName;
    public abstract void AcquireItem(PlayerView player);
    public abstract void UseItem(PlayerView player);
}

public class AddStatusItem : ItemBase
{
    PlayerView _owner;
    public override void AcquireItem(PlayerView player)
    {

    }
    public override void UseItem(PlayerView player)
    {

    }
}