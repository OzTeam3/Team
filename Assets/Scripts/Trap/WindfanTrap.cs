using UnityEngine;
using System.Collections.Generic;

public class FanTrap : MonoBehaviour
{
    [Header("바람 설정")]
    [SerializeField] private float _windStrength = 30f;

    private readonly HashSet<Rigidbody> _entitiesInWind = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                _entitiesInWind.Add(playerRb);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                _entitiesInWind.Remove(playerRb);
            }
        }
    }

    private void FixedUpdate()
    {
        if (_entitiesInWind.Count == 0) return;

        Vector3 windForce = transform.forward * _windStrength;

        foreach (Rigidbody rb in _entitiesInWind)
        {
            if (rb != null)
            {
                rb.AddForce(windForce, ForceMode.Force);
            }
        }
    }
}