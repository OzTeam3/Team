using System;
using System.Collections.Generic;
using UnityEngine;

public enum ZoneType
{
    None,
    Stage,
    UnStage
}

//public enum UIRootType
//{
//    BackgroundUI,
//    MainUI,
//    ContentUI,
//    PopupUI,
//    VeryFrontUI
//}

//public enum UIType
//{
//    ESCPopupUI,
//    FadePopupUI,
//    MainHUD,
//    TitleUI,
//    SettingPopupUI,
//    StartPopupUI
//}

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
    public float MoveSpeed;
    public float JumpForce;
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
    public float ActionValue;
    public float KnockbackForce;
    public float Value1;
    public float Value2;
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

    public bool IsLoaded { get; set; } = false;
}

[System.Serializable]
public class UIData : GameDataBase
{
    public string Name;
    public UIRootType UIRootType;
    public string UIType;
    public string PrefabPath;
}
