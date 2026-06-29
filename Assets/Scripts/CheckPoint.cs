using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private bool _isSave = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isSave)
        {
            _isSave = true;

            Vector3 safePosition = transform.position + (Vector3.up * 1f);

            GameManager.Instance.SaveGame(safePosition);

            Debug.Log("체크포인트 저장 완료: " + safePosition);
        }
    }
}
