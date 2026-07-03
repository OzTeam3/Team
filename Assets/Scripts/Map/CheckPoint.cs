using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Vector3 savePosition = transform.position + (Vector3.up * 1f); //수정

        GameManager.Instance.SaveGame(savePosition);
    }
}
