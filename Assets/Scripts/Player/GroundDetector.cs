using System;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    public event Action<bool> OnGroundTriggeredAction;

    private int _triggerCount;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ground"))
        {
            return;
        }

        _triggerCount++;
        OnGroundTriggeredAction?.Invoke(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Ground"))
        {
            return;
        }

        _triggerCount--;

        if (_triggerCount <= 0)
        {
            OnGroundTriggeredAction?.Invoke(false);
        }
    }
}