using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.ResourceManagement.AsyncOperations;

//using 정리
//로그 정리
public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    //시리얼라이즈 없어도된다
    [SerializeField] private Transform _playerTransform;

    private readonly Dictionary<string, GameObject> _spawnedZones = new Dictionary<string, GameObject>();
    private List<ZoneData> _zoneDataList;

    //?????
    private Transform _stageRoot;
    private GameObject _stageParent;


    //리소스 매니저 같게 해주요
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        InitializeMapManager().Forget();

    }

    private void Update()
    {
        if (_zoneDataList == null) { return; }
        CheckPlayerTransform();
    }

    private async UniTask InitializeMapManager()
    {    
        //게임매니저로 뺸다.
        //await UniTask.WaitUntil(() => DataManager.Instance.IsInitialized);
        await DataManager.Instance.LoadAllDatasAsync();

        _zoneDataList = DataManager.Instance.GetAllData<ZoneData>();
        if (_zoneDataList == null)
        {
            Debug.LogError("[MapManager: InitializeMapManager] 데이터를 가져오지 못했습니다.");
            return;
        }

        //하드코딩 풀어주세요
        _stageParent = new GameObject("StageRoot");

        foreach (ZoneData zone in _zoneDataList)
        {
            //분리좀해주세요
            GameObject zoneObj = await ResourceManager.Instance.InstantiateGameObjectAsync(AddressableUtil.AddressPath.GetZonePath(zone.Name));

            //얼리리턴
           //Obj 안쓰기
            if (zoneObj != null)
            {
                //구조적 수정 (타입으로 관리해도 되는거 아닌가?)
                //하드코딩 풀어주세요

                if (zone.Name.Contains("Stage"))
                {
                    zoneObj.transform.SetParent(_stageParent.transform);
                    zoneObj.transform.position = new Vector3(-341.21f, 335.75f, 477.65f);
                }
                else
                {
                    zoneObj.transform.position = new Vector3();
                }

                _spawnedZones[zone.Name] = zoneObj;
                zoneObj.SetActive(false);
                zone.IsLoaded = false;

                if (zone.Name.Contains("Ground"))
                {
                    zoneObj.SetActive(true);
                    zone.IsLoaded = true;
                }
            }
        }
    }

    private void CheckPlayerTransform()
    {
        if (_playerTransform == null)
        {
            return;
        }

        //거리로 합시다.
        float playerY = _playerTransform.position.y;
        //Vector3.Distance(); 이 방법있고 최적화 : 제곱근 쓰는 방법있어여 (도전영역)

        foreach (ZoneData zone in _zoneDataList)
        {
            if (zone.Name.Contains("Stage"))
            {

                bool isInside = (playerY >= (zone.MinY - 10)) && playerY <= (zone.MaxY + 10);

                if (isInside != zone.IsLoaded)
                {
                    UpdateZoneState(zone, isInside);
                }
            }
        }
    }

    private void UpdateZoneState(ZoneData zone, bool isInside)
    {
        //중괄호 띄어쓰기 워닝이나 에러 출력해주세요
        if (zone.IsLoaded == isInside) return;

        zone.IsLoaded = isInside;

        //얼리리턴
        if (!_spawnedZones.TryGetValue(zone.Name, out GameObject zoneObj))
        {
            zoneObj.SetActive(isInside);
            Debug.Log($"[MapManager: UpdateZoneState] {zone.Name} 활성화 상태: {isInside}");
        }
        else
        {
            Debug.LogWarning($"[MapManager: UpdateZoneState] {zone.Name} 오브젝트를 찾을 수 없습니다.");
        }
    }
}