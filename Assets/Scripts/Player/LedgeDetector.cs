using System;
using UnityEngine;

public class LedgeDetector : MonoBehaviour
{
    public event Action<Collider> OnLedgeTriggeredAction;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ground"))
        {
            return;
        }

        OnLedgeTriggeredAction?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Ground"))
        {
            return;
        }

        OnLedgeTriggeredAction?.Invoke(null);
    }
}
