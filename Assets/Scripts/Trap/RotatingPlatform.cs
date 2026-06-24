using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(Rigidbody))]
public class RotatingPlatformTrap : MonoBehaviour
{
    [Header("회전 설정")]
    [SerializeField] private Vector3 _rotationAxis = Vector3.up; //  회전 방향
    [SerializeField] private float _rotationSpeed = 150.0f; // 회전 스피드

    [Header("플레이어 동기화")]
    [SerializeField] private LayerMask _playerLayer; 
    [SerializeField] private float _maxSurfaceAngle = 45f;

    [SerializeField] private float _maxTrackDistance = 5f;

    private Rigidbody _platformRigidbody;
    private readonly HashSet<Rigidbody> _playersOnPlatform = new();
    private readonly List<Rigidbody> _toUnregister = new();

    private float _minSurfaceDot;

    private void Awake()
    {
        _platformRigidbody = GetComponent<Rigidbody>();

        _platformRigidbody.isKinematic = true;
        _platformRigidbody.useGravity = false;

        _minSurfaceDot = Mathf.Cos(_maxSurfaceAngle * Mathf.Deg2Rad);
    }

    private void FixedUpdate()
    {
        Quaternion deltaRotation = Quaternion.AngleAxis(_rotationSpeed * Time.fixedDeltaTime, _rotationAxis.normalized);

        _platformRigidbody.MoveRotation(_platformRigidbody.rotation * deltaRotation);

        if (_playersOnPlatform.Count == 0)
        {
            return;
        }

        Vector3 pivot = transform.position;

        foreach (Rigidbody playerRb in _playersOnPlatform)
        {
            if (playerRb == null)
            {
                _toUnregister.Add(playerRb);
                continue;
            }
            
            float distFromPivot = Vector3.Distance(playerRb.position, pivot);
            if (distFromPivot > _maxTrackDistance)
            {
                _toUnregister.Add(playerRb);
                continue;
            }

            Vector3 offset = playerRb.position - pivot;
            Vector3 newPos = pivot + deltaRotation * offset;

            playerRb.MovePosition(newPos);
            
        }

        if (_toUnregister.Count > 0)
        {
            foreach (Rigidbody rb in _toUnregister)
            {
                _playersOnPlatform.Remove(rb);
            }
            _toUnregister.Clear();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryRegisterPlayer(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        TryRegisterPlayer(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            _playersOnPlatform.Remove(collision.rigidbody);
        }
    }

    private void TryRegisterPlayer(Collision collision)
    {
        Rigidbody entityRb = collision.rigidbody;
        if (entityRb == null)
        {
            return;
        }

        if (((1 << collision.collider.gameObject.layer) & _playerLayer) == 0)
        {
            return;
        }

        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.down) >= _minSurfaceDot)
            {
                _playersOnPlatform.Add(entityRb);
                return;
            }
        }

        _playersOnPlatform.Remove(entityRb);
    }
}