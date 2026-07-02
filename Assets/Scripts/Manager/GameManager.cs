using Cysharp.Threading.Tasks;
using System.IO;
using System.Threading;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private readonly string PlayerID = "Char_Mj";

    private CharacterData _characterData;

    private PlayerView _playerSaveModel;

    private string _savePath;
    private bool _isPlaying;

    public Player PlayerController { get; private set; }

    public float ElapsedTime { get; private set; }

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
        UIManager.Instance.OpenFadeUI(InitalizeGame, token).Forget();
    }

    private void Update()
    {
        if (!_isPlaying)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenEscapePopup();
        }

        ElapsedTime += Time.deltaTime;
    }

    private void OpenEscapePopup()
    {
        UIManager.Instance.OpenPopupUIAsync(UIType.ESCPopupUI, token).Forget();
    }

    private CancellationToken token = new CancellationToken();
    
    public void InitalizeGame()
    {
        InitallizeLogic().Forget();
    }

    public async UniTask<Player> SettingPlayer()
    {
        if (PlayerController == null)
        {
            await InstantiatePlayerAsync();
        }

        SetPlayerPosition();

        return PlayerController;
    }

    public void PlayNewGame()
    {
        ResetPlaySaveModel();
        ElapsedTime = _playerSaveModel.ElapsedTime;
        _isPlaying = true;
    }

    public void PlayLoadGame()
    {
        LoadGame();
        ElapsedTime = _playerSaveModel.ElapsedTime;
        _isPlaying = true;
    }

    public void EndGame()
    {
        _isPlaying = false;
        PlayerController.gameObject.SetActive(false);
    }

    public void FinishGame()
    {
        EndGame();
        UIManager.Instance.OpenPopupUIAsync(UIType.EndingPopupUI).Forget();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
          Application.Quit();
#endif
    }

    public void SaveGame(Vector3 savePosition)
    {
        if (_playerSaveModel == null)
        {
            Debug.LogError("_currentSaveData가 널입니다");
            return;
        }

        _playerSaveModel.CheckPointPosition = savePosition;
        _playerSaveModel.ElapsedTime = ElapsedTime;
        
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

    public void SetPlayerPosition()
    {
        if (PlayerController == null)
        {
            Debug.LogError("[GameManager:SetPlayerPosition] 플레이어가 없습니다.");
            return;
        }

        PlayerController.transform.position = _playerSaveModel.CheckPointPosition;
    }

    private async UniTask InitallizeLogic()
    {
        await DataManager.Instance.LoadAllDatasAsync(token);
        _characterData = DataManager.Instance.GetData<CharacterData>(PlayerID);
        await UIManager.Instance.OpenMainUIAsync(UIType.TitleUI);
    }

    private async UniTask InstantiatePlayerAsync()
    {
        GameObject player = await ResourceManager.Instance.InstantiateGameObjectAsync(_characterData.PrefabPath, cancellationToken: token);

        if (!player.TryGetComponent(out Player player1))
        {
            Debug.LogError("[GameManager:InstantiatePlayerAsync] 플레이어 컨트롤러 컴포.");
            return;
        }

        //플레이어 초기화

        PlayerController = player1;
    }

    private string GetSavePath()
    {
        if (string.IsNullOrWhiteSpace(_savePath))
        {
            _savePath = Path.Combine(Application.persistentDataPath, "SaveGameData.json");

            if (string.IsNullOrWhiteSpace(_savePath))
            {
                Debug.LogError("[GameManager:GetSavePath] 올바르지 않은 저장 경로입니다.");
                return string.Empty;
            }
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
            Debug.LogError("\"[GameManager:RequestSaveGame] 저장할 세이브 데이터가 없습니다");
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
            ResetPlaySaveModel();
            return;
        }

        string saveData = File.ReadAllText(savePath);
        PlayerView loadPlayerSaveModel = JsonUtility.FromJson<PlayerView>(saveData);

        if (loadPlayerSaveModel == null)
        {
            Debug.LogError("\"[GameManager:RequstLoadGame] 데이터를 읽어오는 데 실패했습니다.");
            return;
        }

        _playerSaveModel = loadPlayerSaveModel;
    }

    public void ResetPlaySaveModel()
    {
        PlayerView newPlayerSaveModel = new PlayerView();

        newPlayerSaveModel.PlayerId = _characterData.Id;
        newPlayerSaveModel.CheckPointPosition = new Vector3(_characterData.StartPositionX, _characterData.StartPositionY, _characterData.StartPositionZ);
        newPlayerSaveModel.ElapsedTime = 0.0f;

        _playerSaveModel = newPlayerSaveModel;
    }
}
