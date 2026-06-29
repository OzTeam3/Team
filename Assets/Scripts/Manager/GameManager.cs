using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    //영어로 바꿔주세요
    [Header("----플레이어관련")]
    [SerializeField] private Rigidbody _playerRigidbody;

    private PlayerView _currentSaveData = new PlayerView();

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

    //디버그 로그 다뺴주세요

    //메서드 (플레이어 동적 생성) 
    //플레이어 동적 생성해서 PlayerView안의 포지션 지정해주는거

    public void SaveGame(Vector3 checkPosition)
    {
        //_currentSaveData 널체크/디버그 에러추가

        _currentSaveData._checkPointPosition = checkPosition;

        RequstSaveGame();
    }


    public void LoadGame()
    {
        _currentSaveData = RequstLoadGame();

        LoadCheckPoint();
    }

    public void ResetGame()
    {
        //위에 필요 없을 수 있다.

        //string path = GetPath();

        //if (File.Exists(path))
        //{
        //    File.Delete(path);
        //}

        _currentSaveData = GetDefaultPlayerData();

        LoadCheckPoint();

        Debug.Log("게임 완전 초기화 및 태초마을로 이동");
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
    public void LoadCheckPoint()
    {
        //이것도 없어질 수도 있다.<< 게임 마지후 체크해봐야됨
        _playerRigidbody.linearVelocity = Vector3.zero;
        _playerRigidbody.angularVelocity = Vector3.zero;

        _playerRigidbody.position = _currentSaveData._checkPointPosition;

        Debug.Log("저장위치로 이동");
    }
    
    private string GetPath()
    {
        //1회만 하도록 수정
        //필드가 string.IsNullOrWhiteSpaces 이게 참이면 
        //거짓이면 필드값 리턴

        return Path.Combine(Application.persistentDataPath, "SaveGameData.json");
    }

    private void RequstSaveGame()
    {
        //시도만 널체크
        string json = JsonUtility.ToJson(_currentSaveData, true);

        //함수안에 함수 쓰지않기
        //GetPath 뺴주세요
        //.json파일이 있는지 없는지 체크해주세요

        File.WriteAllText(GetPath(), json);
        Debug.Log($"저장 완료: {GetPath()}");
    }

    //반환형 void로 수정
    public PlayerView RequstLoadGame()
    {
        string path = GetPath();

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerView data = JsonUtility.FromJson<PlayerView>(json);
            
            //요기서 필드에 저장되게 하자
            //data 널체크 해주셔야됩니다.

            Debug.Log("데이터를 불러왔습니다.");
            return data;
        }
        else
        {
            Debug.LogWarning("세이브 파일이 없습니다. 새 데이터를 생성합니다.");
            return GetDefaultPlayerData();
        }
    }

    //반환형 void로 수정 메서드명 수정 (플레이어뷰 기본값 관련)
    public PlayerView GetDefaultPlayerData()
    {
        //생성된게 없을 때 1회만 실행되도록
        PlayerView newPlayerData = new PlayerView();

        //???
        newPlayerData._playerId = "NoName";

        //시작위치 데이터 드리븐으로 가져와서 Vector3 가져오기 (이주헌)
        newPlayerData._checkPointPosition = new Vector3(0, 1, 0);

        //요기서 필드에 저장되게 하자
        //data 널체크 해주셔야됩니다.
        return newPlayerData;
    }
}
