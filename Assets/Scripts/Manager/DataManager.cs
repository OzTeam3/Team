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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (Instance == this)
        {
            InitializeData().Forget();
        }
    }

    public T GetData<T>(string dataId) where T : GameDataBase
    {
        if (string.IsNullOrWhiteSpace(dataId))
        {
            Debug.LogError($"[DataManger:GetData] {typeof(T).Name} 요청한 ID가 틀렸거나 데이터가 로드되지 않았습니다.");
            return null;
        }

        Type type = typeof(T);
        if (!_dataContainer.TryGetValue(type, out object container) || container is not Dictionary<string, T> dict)
        {
            Debug.LogWarning($"[DataManger:GetData] {type.Name} 컨테이너가 없습니다.");
            return null;
        }

        if (!dict.TryGetValue(dataId, out T data))
        {
            Debug.LogWarning($"[DataManger:GetData] {type.Name} 데이터에 '{dataId}' ID를 가진 데이터가 없습니다.");
            return null;
        }

        return data;
    }

    public List<T> GetAllData<T>() where T : GameDataBase
    {
        Type type = typeof(T);
        if (!_dataContainer.TryGetValue(type, out object container) || container is not Dictionary<string, T> dict)
        {
            Debug.LogWarning($"[DataManger:GetAllData] {type.Name} 컨테이너가 없거나 데이터 구조가 올바르지 않습니다.");
            return new List<T>();
        }

        if (dict == null || dict.Count == 0)
        {
            Debug.LogWarning($"[DataManger:GetAllData] {type.Name} 데이터가 비어있습니다.");
            return new List<T>();
        }

        return dict.Values.ToList();
    }

    public async UniTask LoadAllDatasAsync()
    {

        await LoadDataAsync<CharacterData>(AddressableUtil.AddressPath.Character);
        await LoadDataAsync<ItemData>(AddressableUtil.AddressPath.Item);
        await LoadDataAsync<TrapData>(AddressableUtil.AddressPath.Trap);
        await LoadDataAsync<MonsterData>(AddressableUtil.AddressPath.Monster);
        // 데이터 추가시 여기에 추가
    }

    private async UniTask InitializeData()
    {
        await LoadAllDatasAsync();

        DataManagerTest();

        Debug.Log("[DataManager:InitializeData] 데이터 로드 완료");
    }

    private async UniTask DataManagerTest()
    {
        Debug.Log("[DataManager:Test] 데이터 로딩 끝, 테스트 시작!");

        CharacterData myChar = DataManager.Instance.GetData<CharacterData>("Char_Mj");
        if (myChar != null)
        {
            Debug.Log($"[DataManager:CharacterData] 불러온 캐릭터 이름: {myChar.Name}");
        }
        else
        {
            Debug.LogWarning("[DataManager:CharacterData] myChar이 null입니다. ID가 틀렸거나 데이터가 로드되지 않았습니다.");
        }

        ItemData myItem = DataManager.Instance.GetData<ItemData>("Item_Save_01");
        if (myItem != null)
        {
            Debug.Log($"[DataManager:ItemData] 불러온 아이템 이름: {myItem.Name}");
        }
        else
        {
            Debug.LogWarning("[DataManager:ItemData] myItem이 null입니다. ID가 틀렸거나 데이터가 로드되지 않았습니다.");
        }

        TrapData myTrap = DataManager.Instance.GetData<TrapData>("Trap_SpinCross_001");
        if (myTrap != null)
        {
            Debug.Log($"[DataManager:TrapData] 불러온 트랩 이름: {myTrap.Name}");
        }
        else
        {
            Debug.LogWarning("[DataManager:TrapData] myTrap이 null입니다. ID가 틀렸거나 데이터가 로드되지 않았습니다.");
        }

        List<TrapData> allTraps = DataManager.Instance.GetAllData<TrapData>();

        foreach (var trap in allTraps)
        {
            Debug.Log($"[DataManager:TrapData] 도감 트랩 이름: {trap.Name}");
        }
    }

    private async UniTask LoadDataAsync<T>(string address) where T : GameDataBase
    {
        Debug.Log($"[DataManager:LoadDataAsync<{typeof(T).Name}>] '{address}' 로드 시도 중");

        TextAsset textAsset = await ResourceManager.Instance.GetAssetAsync<TextAsset>(address);

        try
        {
            string JsonText = textAsset.text;
            string wrappedJson = "{\"items\":" + JsonText + "}";
            SerializationWrapper<T> wrapper = JsonUtility.FromJson<SerializationWrapper<T>>(wrappedJson);

            if (wrapper != null && wrapper.items != null)
            {
                _dataContainer[typeof(T)] = wrapper.items.ToDictionary(item => item.Id.ToString());
                Debug.Log($"[DataManger:LoadDataAsync<{typeof(T).Name}>] 데이터를 {wrapper.items.Count}개 로드했습니다.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[DataManger:LoadDataAsync<{typeof(T).Name}> JSON 변환 오류] {ex.Message}");
        }
    }

    [Serializable]
    private class SerializationWrapper<T> { public List<T> items; }
}