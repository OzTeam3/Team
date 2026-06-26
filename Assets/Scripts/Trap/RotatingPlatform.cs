using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RotatingPlatformTrap : MonoBehaviour
{
    [Header("회전 설정")]
    [SerializeField] private Vector3 _rotationAxis = Vector3.up;

    private Rigidbody _platformRigidbody;
    private float _maxSurfaceAngle;
    private float _rotationSpeed;

    private void Awake()
    {
        if (!TryGetComponent<Rigidbody>(out _platformRigidbody))
        {
            Debug.LogError("[RotatingPlatformTrap] Rigidbody 컴포넌트가 없습니다.");
            return;
        }

        _platformRigidbody.isKinematic = true;
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
        if (!collision.gameObject.CompareTag("Player")) return;
        if (!collision.gameObject.TryGetComponent(out Rigidbody playerRb)) return;

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

            Quaternion deltaRotation = Quaternion.AngleAxis(_rotationSpeed * Time.fixedDeltaTime, _rotationAxis.normalized);
            Vector3 newPos = pivot + (deltaRotation * offset);

            playerRb.MovePosition(newPos);
        }
    }
}