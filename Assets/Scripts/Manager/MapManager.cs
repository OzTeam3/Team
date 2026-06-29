using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [SerializeField] private Transform _playerTransform;

    private Dictionary<string, GameObject> _spawnedZones = new Dictionary<string, GameObject>();
    private List<ZoneData> _zoneDataList;

    private Transform _stageRoot;
    private GameObject _stageParent;

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
        //await UniTask.WaitUntil(() => DataManager.Instance.IsInitialized);
        await DataManager.Instance.LoadAllDatasAsync();
        _zoneDataList = DataManager.Instance.GetAllData<ZoneData>();
        if (_zoneDataList == null)
        {
            Debug.LogError("[MapManager: InitializeMapManager] 데이터를 가져오지 못했습니다.");
            return;
        }

        _stageParent = new GameObject("StageRoot");

        foreach (ZoneData zone in _zoneDataList)
        {
            GameObject zoneObj = await ResourceManager.Instance.InstantiateGameObjectAsync(AddressableUtil.AddressPath.GetZonePath(zone.Name));

            if (zoneObj != null)
            {
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

        float playerY = _playerTransform.position.y;

        foreach (ZoneData zone in _zoneDataList)
        {
            if (zone.Name.Contains("Stage"))
            {
                bool isInside = playerY >= (zone.MinY - 10) && playerY <= (zone.MaxY + 10);

                if (isInside != zone.IsLoaded)
                {
                    UpdateZoneState(zone, isInside);
                }
            }
        }
    }

    private void UpdateZoneState(ZoneData zone, bool isInside)
    {
        if (zone.IsLoaded == isInside) return;
        zone.IsLoaded = isInside;

        if (_spawnedZones.TryGetValue(zone.Name, out GameObject zoneObj))
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