using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DataTest : MonoBehaviour
{
    private void Start()
    {
        AddDataTest().Forget();
    }

    private async UniTaskVoid AddDataTest()
    {
        Debug.Log("[Test] 초기화 대기");

        try {
            await UniTask.WaitUntil(IsDataManagerAddReady)
                .Timeout(TimeSpan.FromSeconds(10));

            Debug.Log("[Test] 데이터 로딩 끝, 테스트 시작!");

            CharacterData myChar = DataManager.Instance.GetDataAdd<CharacterData>("Char_Mj");
            if (myChar != null)
            {
                Debug.Log($"불러온 캐릭터 이름: {myChar.Name}");
            }
            else
            {
                Debug.LogWarning("[Fail] myChar이 null입니다.");
            }

            ItemData myItem = DataManager.Instance.GetDataAdd<ItemData>("Item_Save_01");
            if (myItem != null)
            {
                Debug.Log($"불러온 아이템 이름: {myItem.Name}");
            }
            else
            {
                Debug.LogWarning("[Fail] myItem이 null입니다. ID가 틀렸거나 데이터가 로드되지 않았습니다.");
            }

            TrapData myTrap = DataManager.Instance.GetDataAdd<TrapData>("Trap_SpinCross_001");
            if (myTrap != null)
            {
                Debug.Log($"불러온 트랩 이름: {myTrap.Name}");
            }
            else
            {
                Debug.LogWarning("[Fail] myTrap이 null입니다. ID가 틀렸거나 데이터가 로드되지 않았습니다.");
            }
        }
        catch (TimeoutException)
        {
            Debug.LogError("[Test] 타임아웃! DataManager가 10초 이내에 준비되지 않았습니다. Addressables 설정을 확인하세요.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Test] 예기치 않은 오류 발생: {ex.Message}");
        }
    }
 
private bool IsDataManagerAddReady()
    {
        Debug.Log("Checking...");

        return DataManager.Instance != null && DataManager.Instance.IsInitialized;
    }
}
