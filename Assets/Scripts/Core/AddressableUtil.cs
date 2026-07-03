public class AddressableUtil
{
    public static class DataPath
    {
        public const string Character = "JsonOutput/Character";
        public const string Item = "JsonOutput/Item";
        public const string StatUpItem = "JsonOutput/StatUpItem";
        public const string Trap = "JsonOutput/Trap";
        public const string Monster = "JsonOutput/Monster";
        public const string Zone = "JsonOutput/Zone";
        public const string UI = "JsonOutput/UI";
        public const string GameSetting = "JsonOutput/GameSetting";
        public const string Sound = "JsonOutput/Sound";
    }

    public static string GetUIPath(string uiName)
    {
        return $"JsonOutput/{uiName}";
    }
}
