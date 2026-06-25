using Cysharp.Threading.Tasks;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _BGMSource; // 배경음용
    [SerializeField] private AudioSource _audioSource; // 효과음용

    public static SoundManager Instance { get; set; }
    public float BGMVolume
    {
        get { return _BGMSource.volume; }
        set { _BGMSource.volume = value; }
    }
    public float SFXVolume
    {
        get { return _audioSource.volume; }
        set { _audioSource.volume = value; }
    }

    private void Awake()
    {
        Instance = this;
    }

    public string GetSoudPath(string soundDataId)
    {
        string path = soundDataId;
        return path;
    }

    public void PlayBGM(string soundDataId)
    {
        LoadAndPlayAudioClip(_BGMSource, soundDataId).Forget();
    }

    public void PlaySFX(string soundDataId)
    {
        LoadAndPlayAudioClip(_audioSource, soundDataId).Forget();
    }

    public void StopBGM()
    {
        _BGMSource.Stop();
    }

    public void StopSFX()
    {
        _audioSource.Stop();
    }

    // 어드레서블 불러오기 함수
    public static async UniTaskVoid LoadAndPlayAudioClip(AudioSource audioSource, string audioPath, bool isLoop = false)
    {
        AudioClip clip = await ResourceManager.Instance.GetAssetAsync<AudioClip>(audioPath);
        if (clip == null)
        {
            Debug.LogError($"{audioPath}를 찾을 수 없습니다! 어드레서블 설정이 되어 있는지 확인해주세요.");
            return;
        }

        Debug.Log($"[SoundManager] 클립 로드 성공: {clip.name}, AudioSource volume: {audioSource.volume}, mute: {audioSource.mute}");

        if (isLoop == true)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
