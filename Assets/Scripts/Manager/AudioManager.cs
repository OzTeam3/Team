using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static async UniTask<bool> PlayClip(AudioSource audioSource, string audioDataid, bool isLoop = false, CancellationToken cancellationToken = default)
    {
        SoundData soundData = DataManager.Instance.GetData<SoundData>(audioDataid);

        if (soundData == null)
        {
            Debug.LogError($"[AudioManager:PlayClip] 오디오 데이터를 가져오지 못했습니다.");
            return false;
        }

        AudioClip clip = await ResourceManager.Instance.GetAssetAsync<AudioClip>(soundData.PrefabPath, cancellationToken);
        if (clip == null)
        {
            Debug.LogError($"[AudioManager:PlayClip] 오디오 클립 어드레서블 설정이 되어 있는지 확인해주세요.");
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
