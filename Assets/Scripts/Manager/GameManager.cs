using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("----Player Settings")]
    private GameObject _playerPrefab;
    private Rigidbody _playerRigidbody;


    private PlayerView _currentSaveData = new PlayerView();
    private string _savePath;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[GameManager:Awake] 현재 인스턴스가 존재하여 중복 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        SetPlayerData();
    }

    //디버그 로그 다뺴주세요

    //메서드 (플레이어 동적 생성) 
    //플레이어 동적 생성해서 PlayerView안의 포지션 지정해주는거
    public void InitPlayer()
    {
        if (_playerPrefab == null || _currentSaveData == null)
        {
            Debug.LogError("[GameManager]프리팹 혹은 세이브 데이터가 없어서 플레이어를 생성 할 수 없습니다");
            return;
        }

        GameObject playerObject = Instantiate(_playerPrefab, _currentSaveData._checkPointPosition, Quaternion.identity);
        _playerRigidbody = playerObject.GetComponent<Rigidbody>();
        _playerPrefab = playerObject.GetComponent<GameObject>();
    }
    public void SaveGame(Vector3 checkPosition)
    {
        //_currentSaveData 널체크/디버그 에러추가
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
        SetPlayerData();
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


    //메서드명 (플레이어 위치 변경)과 관련된 이름으로 바꾸자
    public void SetPlayerPosition()
    {
        if (_playerRigidbody == null) return;
        //이것도 없어질 수도 있다.<< 게임 마지후 체크해봐야됨
        _playerRigidbody.linearVelocity = Vector3.zero;
        _playerRigidbody.angularVelocity = Vector3.zero;

        _playerRigidbody.position = _currentSaveData._checkPointPosition;

    }
    
    private string GetPath()
    {
        if (string.IsNullOrWhiteSpace(_savePath))
        {
          _savePath =  Path.Combine(Application.persistentDataPath, "SaveGameData.json");
        }
        return _savePath;
    }

    private void RequstSaveGame()
    {
        string path = GetPath();
        //시도만 널체크
        string json = JsonUtility.ToJson(_currentSaveData, true);


        if (string.IsNullOrEmpty(json)) 
        {
            Debug.LogError("세이브 파일이 없습니다");
            return;
        }

        File.WriteAllText(path, json);
    }

    //반환형 void로 수정
    public void RequstLoadGame()
    {
        string path = GetPath();

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerView data = JsonUtility.FromJson<PlayerView>(json);

            //요기서 필드에 저장되게 하자
            //data 널체크 해주셔야됩니다.
            _currentSaveData = data;

            if (data == null) 
            {
                Debug.LogError("데이터를 읽어오는데 실패했습니다");
                return;
            }

            Debug.Log("데이터를 불러왔습니다.");
        }
        else
        {
            Debug.LogWarning("세이브 파일이 없습니다. 새 데이터를 생성합니다.");
        }
    }

    //반환형 void로 수정 메서드명 수정 (플레이어뷰 기본값 관련)
    public void SetPlayerData()
    {
        if (_currentSaveData != null) return;
        PlayerView newPlayerData = new PlayerView();

        newPlayerData._playerId = "NoName";

        //시작위치 데이터 드리븐으로 가져와서 Vector3 가져오기 (이주헌)
        newPlayerData._checkPointPosition = new Vector3(0, 1, 0);

        //요기서 필드에 저장되게 하자
        //data 널체크 해주셔야됩니다.
        if (newPlayerData != null)
        {
            _currentSaveData = newPlayerData;
        }
    }
}
