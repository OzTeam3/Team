using UnityEngine;

public class FanBladeSpinner : MonoBehaviour
{
    [SerializeField] private float _spinSpeed = 720f;

    private void Update()
    {
        transform.Rotate(0f, 0f, _spinSpeed * Time.deltaTime);
    }
}