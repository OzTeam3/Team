using System;
using UnityEngine;

//확인용 로그 전부 제거
//이벤트도 맞는데 액션으로 바꿔주세요 On붙혀주세요
//그라운드로 태그 바꾸기
//얼리리턴으로 바꾸기
public class LedgeDetector : MonoBehaviour
{
    public event Action<Collider> LedgeTriggeredEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ledge")) // 태그 그라운드로 
        {
            //모서리인지 아닌지 판단
            Debug.Log("충돌중");
            LedgeTriggeredEvent?.Invoke(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ledge"))
        {
            LedgeTriggeredEvent?.Invoke(null); // 괄호안에 널투입
        }
    }
}

//그라운드 확인 -콜라이더 가져오기 -  모서리 알아서 -  