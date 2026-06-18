using UnityEngine;

public class GatewaySpot : MonoBehaviour
{
    [SerializeField] private Transform _transformGatewayPoint;
    [SerializeField] private Vector3 _positionArrivalPoint;
    [SerializeField] private Vector3 _rotationArrivalRotation;

    [SerializeField] private string ArrivalZoneDataId;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var arrivalPoint = _transformGatewayPoint == null ? _positionArrivalPoint : _transformGatewayPoint.position;
            var arrivalRotation = _rotationArrivalRotation;
            MovePlayerToOtherPosition(other.transform, arrivalPoint, arrivalRotation);
        }
    }

    private void MovePlayerToOtherPosition(Transform playerTransform, Vector3 targetPosition, Vector3 targetRotation)
    {
        playerTransform.position = targetPosition;
        playerTransform.Rotate(targetRotation);
    }
}
