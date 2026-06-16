using UnityEngine;

public class SpinTrap : MonoBehaviour
{
    [SerializeField] private float _spinSpeed = 60f;

    //[SerializeField] private Rigidbody _trapRigidbody;

    private void Awake()
    {
        //if (GetComponent<Rigidbody>() != null)

        //{

        //    _trapRigidbody = GetComponent<Rigidbody>();

        //}
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * _spinSpeed * Time.deltaTime);
    }

    //private void FixedUpdate()
    //{
    //    Quaternion deltaSpin = Quaternion.Euler(Vector3.up * _spinSpeed *  Time.fixedDeltaTime);

    //    _trapRigidbody.MoveRotation(_trapRigidbody.rotation * deltaSpin);
    //}
}
