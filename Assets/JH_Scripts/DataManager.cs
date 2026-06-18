using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    private readonly Dictionary<Type, object> _dataContainer = new Dictionary<Type, object>();

    public bool IsInitialized { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            InitializeData().Forget();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async UniTaskVoid InitializeData()
    {
        var initHandle = Addressables.InitializeAsync();
        await initHandle.ToUniTask();

        if (initHandle.Status == AsyncOperationStatus.Succeeded)
        {
            await LoadAllDatasAsync();
            IsInitialized = true;
            Debug.Log("[DataManager] 모든 데이터 로드 완료");
        }
        else
        {
            Debug.LogError("[DataManager] Addressables 초기화 실패!");
        }
    }

    public T GetDataAdd<T>(string dataId) where T : GameDataBase
    {
        if (string.IsNullOrEmpty(dataId))
        {
            Debug.LogError($"[{typeof(T).Name}] 요청한 ID가 틀렸거나 데이터가 로드되지 않았습니다.");
            return null;
        }

        Type type = typeof(T);

        if (!_dataContainer.TryGetValue(type, out object container))
        {
            Debug.LogWarning($"[Warning] {type.Name} 컨테이너가 없습니다.");
            return null;
        }

        var dict = (Dictionary<string, T>)container;

        if (dict.TryGetValue(dataId, out T data))
        {
            return data;
        }

        Debug.LogWarning($"[Warning] {type.Name} 데이터에 '{dataId}' ID를 가진 데이터가 없습니다.");
        return null;
    }

    private async UniTask LoadAllDatasAsync()
    {
        await UniTask.WhenAll
        (
            LoadDataAsync<CharacterData>("JsonOutput/Character"),
            LoadDataAsync<ItemData>("JsonOutput/Item"),
            LoadDataAsync<TrapData>("JsonOutput/Trap")
            // 데이터 추가시 여기에 추가
        );
    }

    [Serializable]
    private class SerializationWrapper<T> { public List<T> items; }

    private async UniTask LoadDataAsync<T>(string address) where T : GameDataBase
    {
        Debug.Log($"[DataManager] Addressables를 통해 '{address}' 로드 시도 중");

        TextAsset textAsset = await Addressables.LoadAssetAsync<TextAsset>(address).ToUniTask();

        try
        {
            string JsonText = textAsset.text;
            string wrappedJson = "{\"items\":" + JsonText + "}";
            SerializationWrapper<T> wrapper = JsonUtility.FromJson<SerializationWrapper<T>>(wrappedJson);

            if (wrapper != null && wrapper.items != null)
            {
                _dataContainer[typeof(T)] = wrapper.items.ToDictionary(item => item.Id.ToString());
                Debug.Log($"{typeof(T).Name} 데이터를 {wrapper.items.Count}개 로드했습니다.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{typeof(T).Name} JSON 변환 오류] {ex.Message}");
        }
    }
}