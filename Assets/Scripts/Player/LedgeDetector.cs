using System;
using UnityEngine;

public class LedgeDetector : MonoBehaviour
{
    public event Action<Collider> LedgeTriggeredEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ledge")) // 태그 그라운드로 
        {
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