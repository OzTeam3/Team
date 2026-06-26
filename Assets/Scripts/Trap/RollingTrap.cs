using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RollingLogTrap : MonoBehaviour
{
    [Header("통나무 회전 설정")]
    [SerializeField] private Vector3 _rotationAxis = Vector3.right;

    private Rigidbody _platformRigidbody;
    private float _maxSurfaceAngle = 45f;
    private float _rotationSpeed = 80;

    private void Awake()
    {
        if (!TryGetComponent<Rigidbody>(out _platformRigidbody))
        {
            Debug.LogError("[RollingLogTrap] Rigidbody 컴포넌트가 없습니다.");
            return;
        }
    }

    private void FixedUpdate()
    {
        Quaternion deltaRotation = Quaternion.AngleAxis(_rotationSpeed * Time.fixedDeltaTime, _rotationAxis.normalized);
        _platformRigidbody.MoveRotation(_platformRigidbody.rotation * deltaRotation);
    }

    public void Init(string trapId)
    {
        TrapData data = DataManager.Instance.GetData<TrapData>(trapId);
        if (data == null)
        {
            Debug.Log($"{trapId}에 해당하는 데이터가 없습니다.");
            return;
        }

        _rotationSpeed = data.ActionValue;
        _maxSurfaceAngle = data.Value1;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        if (!collision.gameObject.TryGetComponent(out Rigidbody playerRb))
        {
            return;
        }

        bool isPlayerOnTop = false;

        float minSurfaceDot = Mathf.Cos(_maxSurfaceAngle * Mathf.Deg2Rad);

        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.down) >= minSurfaceDot)
            {
                isPlayerOnTop = true;
                break;
            }
        }

        if (isPlayerOnTop)
        {
            Vector3 pivot = transform.position;
            Vector3 offset = playerRb.position - pivot;

            Vector3 surfaceDirection = Vector3.Cross(_rotationAxis.normalized, offset);
            Vector3 moveDelta = surfaceDirection * (_rotationSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime);

            playerRb.MovePosition(playerRb.position + moveDelta);
        }
    }
}