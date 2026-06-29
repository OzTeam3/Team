using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressableUtil
{
    public static class AddressPath
    {
        public const string Character = "JsonOutput/Character";
        public const string Item = "JsonOutput/Item";
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
