using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Player _player;

    private readonly string PlayerID = "Char_Mj";

    public event Action<Player> PlayerCreated;

    private CharacterData _characterData;

    private PlayerView _playerSaveModel;

    private string _savePath; //확인

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[GameManager:Awake] 현재 인스턴스가 존재하여 중복 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {

        StartUI(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private CancellationToken token = new CancellationToken();
    public void Initallize()
    {
        _characterData = DataManager.Instance.GetData<CharacterData>(PlayerID);
    }

    public async UniTask InstantiatePlayerAsync()
    {
        GameObject player = await ResourceManager.Instance.InstantiateGameObjectAsync(_characterData.PrefabPath, cancellationToken: token);

        if(!player.TryGetComponent(out Player player1))
        {
            Debug.LogError("ddd");
            return;
        }

        //플레이어 초기화

        _player = player1;
        PlayerCreated?.Invoke(_player);
    }

    public void SaveGame(Vector3 savePosition)
    {
        if (_playerSaveModel == null)
        {
            Debug.LogError("_currentSaveData가 널입니다");
            return;
        }

        _playerSaveModel._checkPointPosition = savePosition;
        
        bool isRequestSuccess = RequestSaveGame();

        if(!isRequestSuccess)
        {
            Debug.LogError("");
        }
    }
    
    public void LoadGame()
    {
        RequstLoadGame();
    }


    //리셋을 할까?
    public void ResetGame()
    {
        SetDefaultPlayerData();
        SetPlayerPosition();
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
        if (_player == null)
        {
            Debug.LogError("");
            return;
        }

        //_playerRigidbody.linearVelocity = Vector3.zero;
        //_playerRigidbody.angularVelocity = Vector3.zero;
        //_playerRigidbody.position = _playerSaveModel._checkPointPosition;
    }

    private string GetSavePath()
    {
        _savePath = Path.Combine(Application.persistentDataPath, "SaveGameData.json");

        if (string.IsNullOrWhiteSpace(_savePath))
        {
            Debug.LogError("");
            return string.Empty;
        }

        return _savePath;
    }

    private bool RequestSaveGame()
    {
        string savePath = GetSavePath();

        if (string.IsNullOrWhiteSpace(savePath))
        {
            Debug.LogError("");
            return false;
        }

        string saveData = JsonUtility.ToJson(_playerSaveModel, true);

        if (string.IsNullOrWhiteSpace(saveData))
        {
            Debug.LogError("저장할 세이브 데이터가 없습니다");
            return false;
        }

        File.WriteAllText(savePath, saveData);
        return true;
    }

    private void RequstLoadGame()
    {
        string savePath = GetSavePath();

        if (!File.Exists(savePath))
        {
            SetDefaultPlayerData();
            return;
        }

        string saveData = File.ReadAllText(savePath);
        PlayerView loadPlayerSaveModel = JsonUtility.FromJson<PlayerView>(saveData);

        if (loadPlayerSaveModel == null)
        {
            Debug.LogError("데이터를 읽어오는 데 실패했습니다.");
            return;
        }

        _playerSaveModel = loadPlayerSaveModel;
    }

    public void SetDefaultPlayerData()
    {
        PlayerView newPlayerSaveModel = new PlayerView();

        newPlayerSaveModel._playerId = _characterData.Id;
        newPlayerSaveModel._checkPointPosition = new Vector3(_characterData.StartPositionX, _characterData.StartPositionY, _characterData.StartPositionZ);

        _playerSaveModel = newPlayerSaveModel;
    }

    private async UniTaskVoid StartUI(CancellationToken cancellationToken)
    {
       // UIManager.Instance.OpenFadeUI(cancellationToken).Forget();
       //await UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.TitleUI, cancellationToken: cancellationToken);
    }
}
