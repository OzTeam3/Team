using UnityEngine;

public class Shadow : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;

    private void Update()
    {
        transform.position = new Vector3(_playerTransform.position.x, 0.1f, _playerTransform.position.z);
    }
}
