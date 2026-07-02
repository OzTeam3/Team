using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("----Player Settings")]
    private GameObject _playerPrefab;
    private Rigidbody _playerRigidbody;
    private Transform _playerTransform;
    private GameObject _playerObject;

    [Header("----Data Keys")]
    private readonly string StartPosionID = "StartPosition_001";
    private readonly string PlayerID = "Char_Mj";

    public event Action<Transform> PlayerCreated;
    public CharacterData _characterData;
    private PlayerView _currentSaveData;

    private string _savePath;

    public Dictionary<string, CharacterData> characterDict = new Dictionary<string, CharacterData>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[GameManager:Awake] 현재 인스턴스가 존재하여 중복 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadJsonData();
        RequstLoadGame();

        if (_currentSaveData == null)
        {
            SetDefaultPlayerData();
        }
    }

    private void Start()
    {
        InitPlayer();
        StartUI(this.GetCancellationTokenOnDestroy()).Forget();
    }

    public void InitPlayer()
    {

        if (_playerPrefab == null)
        {
             if (characterDict.TryGetValue(PlayerID, out CharacterData playerData))
            {
                _playerPrefab = Resources.Load<GameObject>(playerData.PrefabPath);
            }
        }

        if (_playerPrefab == null || _currentSaveData == null)
        {
            Debug.LogError("[GameManager] 프리팹 혹은 세이브 데이터가 없어서 플레이어를 생성 할 수 없습니다");
            return;
        }

        _playerObject = Instantiate(_playerPrefab, _currentSaveData._checkPointPosition, Quaternion.identity);
        _playerRigidbody = _playerObject.GetComponent<Rigidbody>();
        _playerTransform = _playerObject.GetComponentInChildren<Transform>();

        PlayerCreated?.Invoke(_playerTransform);

    }

    public void SaveGame(Vector3 checkPosition)
    {
        if (_currentSaveData == null)
        {
            Debug.LogError("_currentSaveData가 널입니다");
            return;
        }

        _currentSaveData._checkPointPosition = checkPosition;
        RequstSaveGame();
    }

    public void LoadGame()
    {
        RequstLoadGame();
        SetPlayerPosition();
    }

    public void ResetGame()
    {
        _currentSaveData = null;
        SetDefaultPlayerData();
        SetPlayerPosition();
        if (_playerObject != null)
        {
            Destroy(_playerObject);
        }
    }


    public void ExitGame()
    {
        #if UNITY_EDITOR
          UnityEditor.EditorApplication.isPlaying = false;
        #else
          Application.Quit();
        #endif
    }

    public void SetPlayerPosition()
    {
        if (_playerRigidbody == null) return;

        _playerRigidbody.linearVelocity = Vector3.zero;
        _playerRigidbody.angularVelocity = Vector3.zero;
        _playerRigidbody.position = _currentSaveData._checkPointPosition;
    }

    private string GetPath()
    {
        if (string.IsNullOrWhiteSpace(_savePath))
        {
            _savePath = Path.Combine(Application.persistentDataPath, "SaveGameData.json");
        }
        return _savePath;
    }

    private void RequstSaveGame()
    {
        string path = GetPath();

        string json = JsonUtility.ToJson(_currentSaveData, true);

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("저장할 세이브 데이터가 없습니다");
            return;
        }

        File.WriteAllText(path, json);
    }

    public void RequstLoadGame()
    {
        string path = GetPath();

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerView data = JsonUtility.FromJson<PlayerView>(json);

            if (data == null)
            {
                Debug.LogError("파일은 있지만 데이터를 읽어오는 데 실패했습니다.");
                return;
            }

            _currentSaveData = data;

        }
    }

    private void LoadJsonData()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>("Character");
        

        if (jsonAsset != null)
        {
            List<CharacterData> tempList = JsonConvert.DeserializeObject<List<CharacterData>>(jsonAsset.text);

            characterDict.Clear();
            foreach (CharacterData data in tempList)
            {
                characterDict.Add(data.Id, data);
            }

        }
        else
        {
            Debug.LogError("JSON 파일을 찾을 수 없습니다.");
        }
    }

    public void SetDefaultPlayerData()
    {
        if (_currentSaveData != null) return;

        PlayerView newPlayerData = new PlayerView();
        newPlayerData._playerId = PlayerID;

        if (characterDict.TryGetValue(StartPosionID, out CharacterData startData))
        {
            newPlayerData._checkPointPosition = new Vector3(
                startData.StartPositionX,
                startData.StartPositionY,
                startData.StartPositionZ
            );
        }
        else
        {
            Debug.LogWarning("시작 지점 데이터를 찾지 못했습니다.");
        }

        if (newPlayerData != null)
        {
            _currentSaveData = newPlayerData;
        }
    }
    private async UniTaskVoid StartUI(CancellationToken cancellationToken)
    {
        UIManager.Instance.OpenFadeUI(cancellationToken).Forget();
        await UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.TitleUI, cancellationToken: cancellationToken);
    }

    public Transform GetPlayerTransform()
    {
        return _playerTransform;
    }
}
