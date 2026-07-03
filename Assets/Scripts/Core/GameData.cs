using System;
using System.Collections.Generic;
using UnityEngine;

public enum ZoneType
{
    None,
    Stage,
    UnStage
}

public enum UIRootType
{
    BackgroundUI,
    MainUI,
    ContentUI,
    PopupUI,
    VeryFrontUI
}

public enum UIType
{
    ESCPopupUI,
    FadePopupUI,
    MainHUD,
    TitleUI,
    SettingPopupUI,
    StartPopupUI,
    EndingPopupUI
}

[Serializable]
public class SerializationWrapper<T>
{
    public List<T> items;
}

[System.Serializable]
public class GameDataBase
{
    public string Id;
}

[System.Serializable]
public class CharacterData : GameDataBase
{
    public string Name;
    public float WalkSpeed;
    public float RotationSpeed;
    public float JumpSpeed;
    public float JumpForce;
    public float StartPositionX;
    public float StartPositionY;
    public float StartPositionZ;
    public string PrefabPath;
}

[System.Serializable]
public class StatUpItemData : GameDataBase
{
    public string StatType;
    public float Value;
    public float Duration;
}

[System.Serializable]
public class ItemData : GameDataBase
{
    public string ItemName;
    public string Description;
    public string ItemType;
    public string MeshId;
}

[System.Serializable]
public class TrapData : GameDataBase
{
    public string Name;
    public float SpinSpeed;
    public float KnockbackForce;
    public float WaitingTime;
    public float RespawnTime;
    public float BounceForce;
    public float MaxAngle;
    public float MaxWindStrength;
    public float MaxDistance;
    public string PrefabPath;
}

[System.Serializable]
public class MonsterData : GameDataBase
{
    public string Name;
    public float MoveSpeed;
    public float ChaseSpeed;
    public float PatrolRadius;
    public float DetectRadius;
    public float AttackRadius;
    public float PushForce;
    public float MinWaitTime;
    public float MaxWaitTime;
    public float AttackCooldown;
    public float ViewAngle;
    public float PatrolRotationSpeed;
    public float ChaseRotationSpeed;
    public float AttackHitBuffer;
    public float StartPositionX;
    public float StartPositionY;
    public float StartPositionZ;
    public string PrefabPath;
}

[System.Serializable]
public class ZoneData : GameDataBase
{
    public string Name;
    public ZoneType Type;
    public float MinY;
    public float MaxY;
    public float PositionY;
    public float OffsetX;
    public float OffsetY;
    public float OffsetZ;
    public string PrefabPath;
    
    public Vector3 StagePosition
    {
         get { return new Vector3(OffsetX, OffsetY, OffsetZ); }
    }
    public bool IsLoaded { get; set; } = false;
}

[System.Serializable]
public class UIData : GameDataBase
{
    public string Name;
    public UIRootType UIRootType;
    public UIType UIType;
    public string PrefabPath;
}

[System.Serializable]
public class SoundData : GameDataBase
{
    public string Name;
    public string PrefabPath;
}

public class GameSettingData : GameDataBase
{
    public string Name;
    public float PositionX;
    public float PositionY;
    public float PositionZ;
    public string PrefabsPath;
}
