using UnityEngine;

public class GatewaySpot : MonoBehaviour
{
    [SerializeField] private Transform _transformGatewayPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }
        if (_transformGatewayPoint == null)
        {
            Debug.LogError("[GatewaySpot] _transformGatewayPoint가 지정되지 않았습니다.");
            return;
        }

        MovePlayerToOtherPosition(other.transform, _transformGatewayPoint.position, _transformGatewayPoint.rotation);
    }

    private void MovePlayerToOtherPosition(Transform playerTransform, Vector3 targetPosition, Quaternion targetRotation)
    {
        playerTransform.SetPositionAndRotation(targetPosition, targetRotation);
    }
}
