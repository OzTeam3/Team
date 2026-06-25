using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    private readonly Dictionary<Type, object> _dataContainer = new Dictionary<Type, object>();

    private readonly HashSet<Type> _targetDataTypes = new HashSet<Type>
    {
        typeof(CharacterData),
        typeof(ItemData),
        typeof(TrapData),
        typeof(MonsterData)
    };

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

    public bool HasData()
    {
        return _dataContainer.Count >= _targetDataTypes.Count;
    }
    public T GetData<T>(string dataId) where T : GameDataBase
    {
        if (string.IsNullOrWhiteSpace(dataId))
        {
            Debug.LogError($"[Error] [{typeof(T).Name}] 요청한 ID가 틀렸거나 데이터가 로드되지 않았습니다.");
            return null;
        }

        Type type = typeof(T);
        if (!_dataContainer.TryGetValue(type, out object container) || container is not Dictionary<string, T> dict)
        {
            Debug.LogWarning($"[Warning] {type.Name} 컨테이너가 없습니다.");
            return null;
        }

        if (!dict.TryGetValue(dataId, out T data))
        {
            Debug.LogWarning($"[Warning] {type.Name} 데이터에 '{dataId}' ID를 가진 데이터가 없습니다.");
            return null;
        }

        return data;
    }

    public List<T> GetAllData<T>() where T : GameDataBase
    {
        Type type = typeof(T);
        if (!_dataContainer.TryGetValue(type, out object container) || container is not Dictionary<string, T> dict)
        {
            Debug.LogWarning($"[Warning] {type.Name} 컨테이너가 없거나 데이터 구조가 올바르지 않습니다.");
            return new List<T>();
        }

        if (dict == null || dict.Count == 0)
        {
            Debug.LogWarning($"[Warning] {type.Name} 데이터가 비어있습니다.");
            return new List<T>();
        }

        return dict.Values.ToList();
    }

    public async UniTask LoadAllDatasAsync()
    {

        await LoadDataAsync<CharacterData>(AddressableUtil.AddressPath.Character);
        await LoadDataAsync<TrapData>(AddressableUtil.AddressPath.Trap);
        await LoadDataAsync<MonsterData>(AddressableUtil.AddressPath.Monster);
        await LoadDataAsync<ItemData>(AddressableUtil.AddressPath.Item);
        await LoadDataAsync<StatUpItemData>(AddressableUtil.AddressPath.ItemStatUp);
        // 데이터 추가시 여기에 추가
    }

    private async UniTask InitializeData()
    {
        await LoadAllDatasAsync();
        Debug.Log("[DataManager] 데이터 로드 완료");
    }

    private async UniTask LoadDataAsync<T>(string address) where T : GameDataBase
    {
        Debug.Log($"[DataManager] '{address}' 로드 시도 중");

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
            Debug.LogError($"[Error] [{typeof(T).Name} JSON 변환 오류] {ex.Message}");
        }
    }

    [Serializable]
    private class SerializationWrapper<T> { public List<T> items; }
}