using UnityEngine;


public class PendulumTrap : MonoBehaviour
{
    [SerializeField] private float _swingSpeed;
    [SerializeField] private float _maxAngle;
    [SerializeField] private float _timeOffset;
    [SerializeField] private Transform _pivotTransform;

    private Vector3 _startPosition;
    private Quaternion _startRotation;

    private void Awake()
    {
        _startPosition = _pivotTransform.position;
        _startRotation = _pivotTransform.rotation;
    }

    private void Update()
    {
        float currentAngle = Mathf.Sin((Time.time + _timeOffset) * _swingSpeed) * _maxAngle;

        _pivotTransform.position = _startPosition;
        _pivotTransform.rotation = _startRotation;

        _pivotTransform.RotateAround(transform.position, transform.forward, currentAngle);
    }
}
