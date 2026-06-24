using UnityEngine;

[System.Serializable]
public class GameDataBase
{
    public string Id;
}

[System.Serializable]
public class CharacterData : GameDataBase
{
    public string Name;
    public int MoveSpeed;
    public int JumpForce;
    public string PrefabPath;
}

/*
[System.Serializable]
public class ItemData : GameDataBase
{
    public string Name;
    public string ItemType;
    public int Regen;
    public string IconPath;
}
*/

[System.Serializable]
public class ItemEffectData : GameDataBase
{
    public string ItemID;
    public string EffectType;
    public string[] StringValues;
    public float[] FloatValues;
}

[System.Serializable]
public class ItemMasterData : GameDataBase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string MaterialPath { get; set; }
}

[System.Serializable]
public class TrapData : GameDataBase
{
    public string Name;
    public int ActionSpeed;
    public int KnockbackForce;
    public int Value;
    public string PrefabPath;
}

[System.Serializable]
public class MonsterData : GameDataBase
{
    public string Name;
    public int MoveSpeed;
    public int ChaseSpeed;
    public int PatrolRadius;
    public int DetectRadius;
    public int AttackRadius;
    public int PushForce;
    public string PrefabPath;
}

