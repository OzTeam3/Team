using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressableUtil
{
    public static class AddressPath
    {
        public const string Character = "JsonOutput/Character";
        public const string Item = "JsonOutput/Item";
        public const string ItemStatUp = "JsonOutput/ItemStatUp";
        public const string Trap = "JsonOutput/Trap";
        public const string Monster = "JsonOutput/Monster";
    }

    public static async UniTask<T> LoadAssetAsync<T>(string address)
    {
        return await Addressables.LoadAssetAsync<T>(address).ToUniTask();
    }
}
