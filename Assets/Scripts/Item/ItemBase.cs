public enum ItemType
{
    None,
    StatUp
}

public abstract class ItemBase
{
    public abstract void InitItem(string itemId);
    public abstract void UseItem(PlayerController player);
}
