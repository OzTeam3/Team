using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("----플레이어관련")]
    [SerializeField] private Rigidbody _playerRigidbody;

    private PlayerView _currentSaveData = new PlayerView();
    public static GameManager Instance { get; set; }

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
    private void Update()
    {
    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        SaveCheckPoint();
    //    }

        if (Input.GetKeyDown(KeyCode.T))
        {
            LoadCheckPoint();
        }
    }
    public void SaveCheckPoint(Vector3 checkPosition)
    {
        _currentSaveData._checkPointPosition = checkPosition;

        GameManager.Instance.SaveGame();

        Debug.Log("위치저장 완료");
    }
    public void ResetGame()
    {
        string path = GetPath();
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        _currentSaveData = GetDefaultPlayerData();

        LoadCheckPoint();

        Debug.Log("게임 완전 초기화 및 태초마을로 이동");
    }


    public void LoadCheckPoint()
    {
        _playerRigidbody.linearVelocity = Vector3.zero;
        _playerRigidbody.angularVelocity = Vector3.zero;

        _playerRigidbody.position = _currentSaveData._checkPointPosition;

        Debug.Log("저장위치로 이동");
    }
    private void InitO()
    {
        
    }
    public void SaveGame()
    {
        //PlayerPrefs.SetFloat("SavePosX", _checkPointPosition.x);
        //PlayerPrefs.SetFloat("SavePosY", _checkPointPosition.y);
        //PlayerPrefs.SetFloat("SavePosZ", _checkPointPosition.z);
        //PlayerPrefs.Save();

        //Debug.Log("데이터 저장 완료");

        RequstSaveData(_currentSaveData);
    }

    public void LoadGame()
    {
        //if (PlayerPrefs.HasKey("SavedPosX"))
        //{
        //    float x = PlayerPrefs.GetFloat("SavePosX");
        //    float y = PlayerPrefs.GetFloat("SavePosY");
        //    float z = PlayerPrefs.GetFloat("SavePosZ");

        //    _checkPointPosition = new Vector3(x, y, z);

        //    LoadCheckPoint();
        //}
        //else
        //{

        //    Debug.Log("저장된 데이터가 없습니다.");
        //}

        _currentSaveData = RequstLoadSaveData();

        LoadCheckPoint();
    }

    

    public void ExitGame()
    {
         #if UNITY_EDITOR

         UnityEditor.EditorApplication.isPlaying = false;

         #else

         Application.Quit();

         #endif
    }

    private string GetPath()
    {
        return Path.Combine(Application.persistentDataPath, "SaveGameData.json");
    }

    public void RequstSaveData(PlayerView data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(), json);
        Debug.Log($"저장 완료: {GetPath()}");
    }

    public PlayerView RequstLoadSaveData()
    {
        string path = GetPath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerView data = JsonUtility.FromJson<PlayerView>(json);
            Debug.Log("데이터를 불러왔습니다.");
            return data;
        }
        else
        {
            Debug.LogWarning("세이브 파일이 없습니다. 새 데이터를 생성합니다.");
            return GetDefaultPlayerData();
        }
    }

    public PlayerView GetDefaultPlayerData()
    {
        PlayerView newPlayerData = new PlayerView();
        newPlayerData._playerId = "NoName";

        newPlayerData._checkPointPosition = new Vector3(0, 1, 0);
        return newPlayerData;
    }
}
