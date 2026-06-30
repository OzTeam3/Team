using UnityEngine;

public class GatewaySpot : MonoBehaviour
{
    [SerializeField] private Transform _transformGatewayPoint;
    
    //안씀
    [SerializeField] private Vector3 _positionArrivalPoint;
    [SerializeField] private Vector3 _rotationArrivalRotation;
    [SerializeField] private string _arrivalZoneDataId;

    //코드 싹 수정
    private void OnTriggerEnter(Collider other)
    {
        //얼리리턴
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 arrivalPoint = _transformGatewayPoint == null ? _positionArrivalPoint : _transformGatewayPoint.position;
            Vector3 arrivalRotation = _rotationArrivalRotation;
           
            MovePlayerToOtherPosition(other.transform, arrivalPoint, arrivalRotation);
        }
    }

    private void MovePlayerToOtherPosition(Transform playerTransform, Vector3 targetPosition, Vector3 targetRotation)
    {
        playerTransform.position = targetPosition;
        playerTransform.Rotate(targetRotation);
    }
}
