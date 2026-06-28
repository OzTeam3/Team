using UnityEngine;

public class SoundChangeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlayBGM("Assets/Sound/BGM2");
        }
        return;
    }
}
