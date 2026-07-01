using System;
using UnityEngine;

//이벤트도 맞는데 액션으로 바꿔주세요 On붙혀주세요
//그냥 태그검사 한번 해주길
public class GroundDetector : MonoBehaviour
{
    public event Action<bool> GroundTriggeredEvent;

    private void OnTriggerEnter(Collider other)
    {
        GroundTriggeredEvent.Invoke(true);
    }

    private void OnTriggerExit(Collider other)
    {
        GroundTriggeredEvent.Invoke(false);
    }
}
