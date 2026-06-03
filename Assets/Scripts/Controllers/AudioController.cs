using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSourceBGM;
    [SerializeField] private AudioSource _audioSourceSFX;

    public void PlayBGM(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            Debug.LogWarning($"[배경음 재생] AudioClip이 null입니다. BGM을 재생할 수 없습니다.");
            return;
        }

        ChangeBGMClip(audioClip);
        PlayBGM();
    }
    public void StopBGM()
    {
        _audioSourceBGM.Stop();
    }

    public void PlaySFX(AudioClip audioClip)
    {
        if(audioClip == null)
        {
            Debug.LogWarning($"[효과음 재생] AudioClip이 null입니다. SFX를 재생할 수 없습니다.");
            return;
        }

        _audioSourceSFX.PlayOneShot(audioClip);
    }

    private void PlayBGM()
    {
        _audioSourceBGM.Play();
    }

    private void ChangeBGMClip(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            Debug.LogWarning($"[배경음 재생] AudioClip이 null입니다. BGM을 재생할 수 없습니다.");
            return;
        }

        _audioSourceBGM.clip = audioClip;
    }
}