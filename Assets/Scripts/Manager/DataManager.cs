using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


//로그 찎는 부분은 다 삭제해주세요
//유니테스크 쓰는부분 캔슬토큰 신경써주세여
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    
    //변수명 수정
    private readonly Dictionary<Type, object> _dataContainer = new Dictionary<Type, object>();

    //리소스매니저 똑같이 해주세요
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Debug.LogWarning($"[DataManager:Awake] 현재 인스턴스가 존재하여 중복 오브젝트를 파괴합니다.");
            Destroy(gameObject);
        }
    }

    //삭제
    private void Start()
    {
        //게임매니저로 빠진다.
        InitializeData().Forget();
    }

    //{}제거
    //무슨 자료인데 dict 수정
    public T GetData<T>(string dataId) where T : GameDataBase
    {
        if (string.IsNullOrWhiteSpace(dataId))
        {
            Debug.LogError($"[DataManger:GetData] {typeof(T).Name} 요청한 ID가 틀렸거나 데이터가 로드되지 않았습니다.");
            return null;
        }

        Type type = typeof(T);

        //두개로 분리
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
        //이걸 해야될까?
        Type type = typeof(T);
        
        if (!_dataContainer.TryGetValue(type, out object container) || container is not Dictionary<string, T> dict)
        {
            Debug.LogWarning($"[DataManger:GetAllData] {type.Name} 컨테이너가 없거나 데이터 구조가 올바르지 않습니다.");

            //리턴 리스트 최적화 << 캐싱
            return new List<T>();
        }

        //카운트 0이 에러인가?
        if (dict == null || dict.Count == 0)
        {
            Debug.LogWarning($"[DataManger:GetAllData] {type.Name} 데이터가 비어있습니다.");
            return new List<T>();
        }

        //이건 생각만 : Linq 사용해야될까?
        //foreach문 으로 처리할 순 없나?
        return dict.Values.ToList();
    }

    public async UniTask LoadAllDatasAsync()
    {
        await LoadDataAsync<CharacterData>(AddressableUtil.AddressPath.Character);
        await LoadDataAsync<ItemData>(AddressableUtil.AddressPath.Item);
        await LoadDataAsync<TrapData>(AddressableUtil.AddressPath.Trap);
        await LoadDataAsync<MonsterData>(AddressableUtil.AddressPath.Monster);
        await LoadDataAsync<ZoneData>(AddressableUtil.AddressPath.Zone);
    }

    //삭제
    private async UniTask InitializeData()
    {
        await LoadAllDatasAsync();

        //DataManagerTest();

        Debug.Log("[DataManager:InitializeData] 데이터 로드 완료");
    }

    private async UniTask LoadDataAsync<T>(string address) where T : GameDataBase
    {
        Debug.Log($"[DataManager:LoadDataAsync<{typeof(T).Name}>] '{address}' 로드 시도 중");

        TextAsset textAsset = await ResourceManager.Instance.GetAssetAsync<TextAsset>(address);

        //널체크 해주세요

        try
        {
            string JsonText = textAsset.text;
            string wrappedJson = "{\"items\":" + JsonText + "}";
            SerializationWrapper<T> wrapper = JsonUtility.FromJson<SerializationWrapper<T>>(wrappedJson);

            if (wrapper != null && wrapper.items != null)
            {
                //람다 뺼수 있을까?
                //_dataContainer[typeof(T)] 있는지 체크 그리고 있으면 워닝 덮어쓴다는 경고문 추가
                _dataContainer[typeof(T)] = wrapper.items.ToDictionary(item => item.Id.ToString());
                Debug.Log($"[DataManger:LoadDataAsync<{typeof(T).Name}>] 데이터를 {wrapper.items.Count}개 로드했습니다.");
            }
        }
        catch
        {
            //오류가 아니라 예외상황 발생
            Debug.LogError($"[DataManger:LoadDataAsync<{typeof(T).Name}> JSON 변환 오류]");
        }
    }

    //시리얼라이즈 데이터 DTO GameDataBase로 뺴쭈세요
    [Serializable]
    private class SerializationWrapper<T> { public List<T> items; }
}