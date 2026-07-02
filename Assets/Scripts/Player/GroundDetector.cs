using System;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    public event Action<bool> OnGroundTriggeredAction;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ground"))
        {
            return;
        }

        OnGroundTriggeredAction?.Invoke(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Ground"))
        {
            return;
        }

        OnGroundTriggeredAction?.Invoke(false);
    }
}