public class AddressableUtil
{
    public static class DataPath
    {
        public const string Character = "JsonOutput/Character";
        public const string Item = "JsonOutput/Item";
        public const string ItemStatUp = "JsonOutput/ItemStatUp";
        public const string Trap = "JsonOutput/Trap";
        public const string Monster = "JsonOutput/Monster";
        public const string Zone = "JsonOutput/Zone";
        public const string UI = "JsonOutput/UI";
        public const string GameSetting = "JsonOutput/GameSetting";
        public const string Sound = "JsonOutput/Sound";
    }

    public static class UIPath
    {
        public const string ESCPopupUI = "JsonOutput/ESCPopupUI";
        public const string FadePopupUI = "JsonOutput/FadePopupUI";
        public const string MainHUD = "JsonOutput/MainHUD";
        public const string TitleUI = "JsonOutput/TitleUI";
        public const string SettingPopupUI = "JsonOutput/SettingPopupUI";
        public const string StartPopupUI = "JsonOutput/StartPopupUI";
        public const string EndingPopupUI = "JsonOutput/EndingPopupUI";
    }

    public static class GameSettingPath
    {
        public const string StartPosition = "JsonOutput/StartPosition";
        public const string FirstSave = "JsonOutput/FirstSave";
        public const string SecondSave = "JsonOutput/SecondSave";
        public const string ThirdSave = "JsonOutput/ThirdSave";
        public const string FinalPosition = "JsonOutput/FinalPosition";

    }

    public static class TrapPath
    {
        public const string SpinObject01 = "Prefab/Prefab_Trap_SpinObject_001";
        public const string BreakingPlatform01 = "Prefab/Prefab_Trap_BreakingPlatform_001";
        public const string BlinkPlatform01 = "Prefab/Prefab_Trap_BlinkPlatform_001";
        public const string JumpPad01 = "Prefab/Prefab_Trap_JumpPad_001";
        public const string RollingObject01 = "Prefab/Prefab_Trap_RollingObject_001";
        public const string RollingPlatform01 = "Prefab/Prefab_Trap_RollingPlatform_001";
        public const string RollingPlatform02 = "Prefab/Prefab_Trap_RollingPlatform_002";
        public const string Pendulum_Axe = "Prefab/Prefab_Trap_Pendulum_Axe";
        public const string Pendulum_Hammer = "Prefab/Prefab_Trap_Pendulum_Hammer";
        public const string WindFan01 = "Prefab/Prefab_Trap_WindFan_001";
    }

    public static string GetUIPath(string uiName)
    {
        return $"JsonOutput/{uiName}";
    }
}
