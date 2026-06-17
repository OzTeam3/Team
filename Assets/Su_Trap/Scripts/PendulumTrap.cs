using UnityEngine;


public class PendulumTrap : MonoBehaviour
{
    [SerializeField] private float _swingSpeed;
    [SerializeField] private float _maxAngle;
    [SerializeField] private float _timeOffset;
    

    private void Update()
    {
        float currentAngle = Mathf.Sin((Time.time + _timeOffset) * _swingSpeed) * _maxAngle;

        transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
}
