using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    
    private readonly Dictionary<Type, object> _dataTableList = new Dictionary<Type, object>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[DataManager:Awake] 현재 인스턴스가 존재하여 중복 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    //삭제
    private void Start()
    {
        //게임매니저로 빠진다.
        InitializeData().Forget();
    }

    public T GetData<T>(string dataId) where T : GameDataBase
    {
        Type type = typeof(T);

        if (string.IsNullOrWhiteSpace(dataId))
        {
            Debug.LogError($"[DataManager:GetData] 요청한 ID가 틀렸거나 빈칸입니다.");
            return null;
        }

        if (!_dataTableList.TryGetValue(type, out object container))
        {
            Debug.LogError($"[DataManager:GetData] 컨테이너가 없습니다.");
            return null;
        }

        if (container is not Dictionary<string, T> dataTable)
        {
            Debug.LogError($"[DataManager:GetData] 데이터 구조가 올바르지 않습니다.");
            return null;
        }

        if (!dataTable.TryGetValue(dataId, out T data))
        {
            Debug.LogError($"[DataManager:GetData] 요청한 ID의 데이터가 없습니다.");
            return null;
        }

        return data;
    }

    public List<T> GetAllData<T>() where T : GameDataBase
    {
        Type type = typeof(T);
        
        if (!_dataTableList.TryGetValue(type, out object container))
        {
            Debug.LogError($"[DataManager:GetAllData] 컨테이너가 없거나 데이터 구조가 올바르지 않습니다.");

            return CollectionCache<T>.EmptyList;
        }

        if (container is not Dictionary<string, T> dataTable)
        {
            Debug.LogError($"[DataManager:GetAllData] 데이터 구조가 올바르지 않습니다.");
            return CollectionCache<T>.EmptyList;
        }

        if (dataTable == null)
        {
            Debug.LogError($"[DataManager:GetAllData] 데이터가 비어있습니다.");
            return CollectionCache<T>.EmptyList;
        }

        if (dataTable.Count == 0)
        {
            Debug.LogWarning($"[DataManager:GetAllData] 데이터가 비어있습니다.");
            return CollectionCache<T>.EmptyList;
        }

        //이건 생각만 : Linq 사용해야될까?
        //foreach문 으로 처리할 순 없나?
        return dataTable.Values.ToList();
    }

    public async UniTask LoadAllDatasAsync(CancellationToken cancellationToken = default)
    {
        await LoadDataAsync<CharacterData>(AddressableUtil.DataPath.Character, cancellationToken);
        await LoadDataAsync<ItemData>(AddressableUtil.DataPath.Item, cancellationToken);
        await LoadDataAsync<TrapData>(AddressableUtil.DataPath.Trap, cancellationToken);
        await LoadDataAsync<MonsterData>(AddressableUtil.DataPath.Monster, cancellationToken);
        await LoadDataAsync<ZoneData>(AddressableUtil.DataPath.Zone, cancellationToken);
        await LoadDataAsync<UIData>(AddressableUtil.DataPath.UI, cancellationToken);
        await LoadDataAsync<SoundData>(AddressableUtil.DataPath.Sound, cancellationToken);
    }

    //삭제 게임매니저 머지후
    private async UniTask InitializeData()
    {
        await LoadAllDatasAsync();
    }

    private async UniTask LoadDataAsync<T>(string address, CancellationToken cancellationToken) where T : GameDataBase
    {
        Type type = typeof(T);
        TextAsset textAsset = await ResourceManager.Instance.GetAssetAsync<TextAsset>(address);
        if (textAsset == null)
        {
            Debug.LogError($"[DataManager: LoadDataAsync] 데이터 로드 실패");
            return;
        }

        try
        {
            string JsonText = textAsset.text;
            string wrappedJson = "{\"items\":" + JsonText + "}";
            SerializationWrapper<T> wrapper = JsonUtility.FromJson<SerializationWrapper<T>>(wrappedJson);
            if (wrapper?.items == null)
            {
                Debug.LogError($"[DataManager: LoadDataAsync] 데이터 파싱 결과가 비어있습니다.");
                return;
            }

            if (_dataTableList.ContainsKey(type))
            {
                Debug.LogWarning($"[DataManager: LoadDataAsync] 데이터가 이미 존재합니다. 덮어씁니다.");
            }

            _dataTableList[type] = CreateDictionary(wrapper.items);
        }
        catch
        {
            Debug.LogError($"[DataManager:LoadDataAsync] 예외상황 발생");
        }
    }

    private Dictionary<string, T> CreateDictionary<T>(List<T> items) where T : GameDataBase
    {
        Dictionary<string, T> dictionary = new Dictionary<string, T>();
        if (items == null)
        {
            Debug.LogError($"[DataManager:CreateDictionary] 리스트가 null입니다.");
            return dictionary;
        }

        foreach (T item in items)
        {
            if (item == null)
            {
                Debug.LogWarning($"[DataManager:CreateDictionary] 리스트에 null 데이터가 포함되어 건너뜁니다.");
                continue;
            }

            dictionary[item.Id.ToString()] = item;
        }

        return dictionary;
    }
}