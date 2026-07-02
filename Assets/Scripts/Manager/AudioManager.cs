using Cysharp.Threading.Tasks;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static async UniTask<bool> PlayClip(AudioSource audioSource, string audioPath, bool isLoop = false)
    {
        AudioClip clip = await ResourceManager.Instance.GetAssetAsync<AudioClip>(audioPath);
        if (clip == null)
        {
            Debug.LogError($"[AudioManager:PlayClip] {audioPath}를 찾을 수 없습니다! 어드레서블 설정이 되어 있는지 확인해주세요.");
            return false;
        }

        if (isLoop)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            audioSource.PlayOneShot(clip);
        }
        return true;
    }

    public static void Stop(AudioSource audioSource)
    {
        audioSource.Stop();
    }
}
