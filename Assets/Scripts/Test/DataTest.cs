using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataTest : MonoBehaviour
{
    public List<CharacterData> AllCharacters;
    private void Start()
    {
        AddDataTest().Forget();
    }

    private async UniTaskVoid AddDataTest()
    {
        Debug.Log("[Test] 초기화 대기");


        await UniTask.WaitUntil(() => DataManager.Instance != null);
        await UniTask.WaitUntil(() => DataManager.Instance.HasData());

        Debug.Log("[Test] 데이터 로딩 끝, 테스트 시작!");

        CharacterData myChar = DataManager.Instance.GetData<CharacterData>("Char_Mj");
        if (myChar != null)
        {
            Debug.Log($"불러온 캐릭터 이름: {myChar.Name}");
        }
        else
        {
            Debug.LogWarning("[Warning] myChar이 null입니다.");
        }
        
        /*
        ItemData myItem = DataManager.Instance.GetData<ItemData>("Item_Save_01");
        if (myItem != null)
        {
            Debug.Log($"불러온 아이템 이름: {myItem.Name}");
        }
        else
        {
            Debug.LogWarning("[Warning] myItem이 null입니다. ID가 틀렸거나 데이터가 로드되지 않았습니다.");
        }
        */

        TrapData myTrap = DataManager.Instance.GetData<TrapData>("Trap_SpinCross_001");
        if (myTrap != null)
        {
            Debug.Log($"불러온 트랩 이름: {myTrap.Name}");
        }
        else
        {
            Debug.LogWarning("[Warning] myTrap이 null입니다. ID가 틀렸거나 데이터가 로드되지 않았습니다.");
        }

        List<TrapData> allTraps = DataManager.Instance.GetAllData<TrapData>();

        foreach (var trap in allTraps)
        {
            Debug.Log($"도감 트랩 이름: {trap.Name}");
        }


    }
}
