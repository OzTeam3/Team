using System;
using UnityEngine;

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
