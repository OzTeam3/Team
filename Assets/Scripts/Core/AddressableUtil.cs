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
    }

    public static class ZonePath
    {
        public const string Ground = "JsonOutput/Ground";
        public const string FirstStage = "JsonOutput/FirstStage";
        public const string SecondStage = "JsonOutput/SecondStage";
        public const string ThirdStage = "JsonOutput/ThirdStage";
        public const string WayUp = "JsonOutput/WayUp";
        public const string Map = "JsonOutput/Map";
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

    public static class SoundPath
    {
        public const string Bgm1 = "JsonOutput/Bgm1";
        public const string Bgm2 = "JsonOutput/Bgm2";
        public const string Title = "JsonOutput/Title";
        public const string Button1 = "JsonOutput/Button1";
        public const string Button2 = "JsonOutput/Button2";
    }

    public static class GameSettingPath
    {
        public const string StartPosition = "JsonOutput/StartPosition";
        public const string FirstSave = "JsonOutput/FirstSave";
        public const string SecondSave = "JsonOutput/SecondSave";
        public const string ThirdSave = "JsonOutput/ThirdSave";
        public const string FinalPosition = "JsonOutput/FinalPosition";

    }
    public static string GetZonePath(string zoneName)
    {
        return $"JsonOutput/{zoneName}";
    }

    public static string GetUIPath(string uiName)
    {
        return $"JsonOutput/{uiName}";
    }
}
