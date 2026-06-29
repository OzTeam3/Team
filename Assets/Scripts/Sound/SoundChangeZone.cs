using UnityEngine;

public class SoundChangeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //얼리리턴
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlayBGM("Assets/Sound/BGM2");
        }
    }
}
