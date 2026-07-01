using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

//using 정리
//로그 정리
public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    private readonly Dictionary<string, GameObject> _spawnedZones = new Dictionary<string, GameObject>();
    private List<ZoneData> _zoneDataList;

    //?????
    private Transform _stageRoot;
    private GameObject _stageParent;
    [SerializeField]private Transform _playerTransform;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[MapManager:Awake] 현재 인스턴스가 존재하여 중복 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
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

    private async UniTask InitializeMapManager(CancellationToken cancellationToken = default)
    {
        //게임매니저로 뺸다. 머지하면 빠짐
        //await UniTask.WaitUntil(() => DataManager.Instance.IsInitialized);
        await DataManager.Instance.LoadAllDatasAsync(cancellationToken);

        //await UniTask.WaitUntil(() => DataManager.Instance != null && DataManager.Instance.LoadAllDatasAsync(cancellationToken).Status == UniTaskStatus.Succeeded);

        _zoneDataList = DataManager.Instance.GetAllData<ZoneData>();
        if (_zoneDataList == null)
        {
            Debug.LogError("[MapManager: InitializeMapManager] 데이터를 가져오지 못했습니다.");
            return;
        }

        _stageParent = new GameObject(AddressableUtil.ZonePath.WayUp);
        if (_stageParent == null)
        {
            Debug.LogError("[MapManager: InitializeMapManager] 오브젝트를 생성하지 못했습니다.");
            return;
        }

        foreach (ZoneData zoneData in _zoneDataList)
        {
            string zonePath = AddressableUtil.GetZonePath(zoneData.Name);
            GameObject zoneObject = await ResourceManager.Instance.InstantiateGameObjectAsync(zonePath, cancellationToken: cancellationToken);
            if (zoneObject == null)
            {
                continue;
            }
            
            //bool isStage = zoneData.Type == ZoneType.Stage; 팀장님한테 질문할것
            bool isStage = zoneData.Name.Contains("Stage");
            if (isStage)
            {
                zoneObject.transform.SetParent(_stageParent.transform);
                zoneObject.transform.position = new Vector3(0, 8f, 0);
            }
            else
            {
                zoneObject.transform.position = Vector3.zero;
            }

            _spawnedZones[zoneData.Name] = zoneObject;

            bool isGround = !isStage;
            zoneObject.SetActive(isGround);
            zoneData.IsLoaded = isGround;
        }
    }

    private void CheckPlayerTransform()
    {
        if (_playerTransform == null)
        {
            return;
        }

        float playerY = _playerTransform.position.y;
        // 게임매니저받으면 혜창님이 해줌

        foreach (ZoneData zoneData in _zoneDataList)
        {
            bool isStage = zoneData.Name.Contains("Stage");
            if (isStage)
            {

                bool isInside = (playerY >= (zoneData.MinY)) && (playerY <= (zoneData.MaxY));

                if (isInside != zoneData.IsLoaded)
                {
                    UpdateZoneState(zoneData, isInside);
                }
            }
        }
    }

    private void UpdateZoneState(ZoneData zoneData, bool isInside)
    {
        if (zoneData.IsLoaded == isInside)
        {
            return;
        }

        zoneData.IsLoaded = isInside;

        if (!_spawnedZones.TryGetValue(zoneData.Name, out GameObject zoneObj))
        {
            Debug.LogWarning($"[MapManager: UpdateZoneState] 오브젝트를 찾을 수 없습니다.");
            return;
        }

        zoneObj.SetActive(isInside);
    }
}