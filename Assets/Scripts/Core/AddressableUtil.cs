
//어드레서블 이주헌님이 다해주세요
public class AddressableUtil
{
    public static class AddressPath
    {
        public const string Character = "JsonOutput/Character";
        public const string Item = "JsonOutput/Item";
        public const string ItemStatUp = "JsonOutput/ItemStatUp";
        public const string Trap = "JsonOutput/Trap";
        public const string Monster = "JsonOutput/Monster";
        public const string Map = "JsonOutput/Map";
        public const string Zone = "JsonOutput/Zone";

        public static string GetZonePath(string zoneName)
        {
            return $"JsonOutput/{zoneName}";
        }
    }
}
