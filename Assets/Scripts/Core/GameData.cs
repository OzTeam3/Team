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

[System.Serializable]
public class ItemData : GameDataBase
{
    public string Name;
    public string ItemType;
    public int Regen;
    public string IconPath;
}

[System.Serializable]
public class TrapData : GameDataBase
{
    public string Name;
    public int SpinSpeed;
    public int KnockbackForce;
    public int WaitingTime;
    public int RespawnTime;
    public int BounceForce;
    public int MaxAngle;
    public int MaxWindStrength;
    public int MaxDistance;
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

[System.Serializable]
public class ZoneData : GameDataBase
{
    public string Name;
    public int MinY;
    public int MaxY;
    public string PrefabPath;

    public bool IsLoaded { get; set; } = false;
}

