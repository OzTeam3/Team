using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    private const float SeamlessOffset = 10.0f;

    private readonly Dictionary<string, GameObject> _spawnedZones = new Dictionary<string, GameObject>();
    private List<ZoneData> _zoneDataList;

    [SerializeField] private GameObject _stageRoot;
    [SerializeField] private Transform _playerTransform;

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
        CheckPlayerTransform();
    }

    private async UniTask InitializeMapManager(CancellationToken cancellationToken = default)
    {
        _zoneDataList = DataManager.Instance.GetAllData<ZoneData>();

        if (_zoneDataList == null)
        {
            Debug.LogError("[MapManager: InitializeMapManager] 데이터를 가져오지 못했습니다.");
            return;
        }

        if (_stageRoot == null)
        {
            Debug.LogError("[MapManager: InitializeMapManager] 오브젝트를 생성하지 못했습니다.");
            return;
        }

        foreach (ZoneData zoneData in _zoneDataList)
        {
            GameObject zoneObject = await ResourceManager.Instance.InstantiateGameObjectAsync(zoneData.PrefabPath, cancellationToken: cancellationToken);
            if (zoneObject == null)
            {
                continue;
            }

            bool isStage = zoneData.Name.Contains(ZoneType.Stage.ToString());
            if (isStage)
            {
                zoneObject.transform.SetParent(_stageRoot.transform);
            }

            zoneObject.transform.position = zoneData.StagePosition;
            _spawnedZones[zoneData.Name] = zoneObject;
            bool isNotGround = !isStage;
            zoneObject.SetActive(isNotGround);
            zoneData.IsLoaded = isNotGround;
        }
    }

    private void CheckPlayerTransform()
    {
        if (_playerTransform == null)
        {
            return;
        }

        float playerY = _playerTransform.position.y;

        foreach (ZoneData zoneData in _zoneDataList)
        {
            bool isStage = zoneData.Name.Contains(ZoneType.Stage.ToString());
            if (isStage)
            {
                bool isInside = (playerY >= (zoneData.MinY - SeamlessOffset)) && (playerY <= (zoneData.MaxY + SeamlessOffset));

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

        if (!_spawnedZones.TryGetValue(zoneData.Name, out GameObject zoneObject))
        {
            Debug.LogWarning($"[MapManager: UpdateZoneState] 오브젝트를 찾을 수 없습니다.");
            return;
        }

        zoneObject.SetActive(isInside);
    }
}