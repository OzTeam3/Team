using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioBGMSource;
    [SerializeField] private AudioSource _audiosfxSource;

    public static AudioController Instance { get; private set; }

    private void OnDisable()
    {
        
    }

    public float BGMVolume
    {
        get { return _audioBGMSource.volume; }
        set { _audioBGMSource.volume = value; }
    }

    public float SFXVolume
    {
        get { return _audiosfxSource.volume; }
        set { _audiosfxSource.volume = value; }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void PlayBGM(string soundDataId, CancellationToken cancellationToken = default)
    {
        AudioManager.PlayClip(_audioBGMSource, soundDataId, true, cancellationToken).Forget();
    }

    public void PlaySFX(string soundDataId, CancellationToken cancellationToken = default)
    {
        AudioManager.PlayClip(_audiosfxSource, soundDataId, false, cancellationToken).Forget();
    }

    public void StopBGM()
    {
        AudioManager.Stop(_audioBGMSource);
    }

    public void StopSFX()
    {
        AudioManager.Stop(_audiosfxSource);
    }
}
