using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioController _audioController;

    private readonly Dictionary<string, AudioClip> _cachedAudioClips = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{gameObject.name}] 이미 AudioManager 인스턴스가 존재하여 생성된 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlayBGM(string audioId)
    {
        AudioClip audioClip = _cachedAudioClips[audioId];
        _audioController.PlayBGM(audioClip);
    }

    public void StopBGM()
    {
        _audioController.StopBGM();
    }

    public void PlaySFX(string audioId)
    {
        AudioClip audioClip = _cachedAudioClips[audioId];
        _audioController.PlaySFX(audioClip);
    }

    public float GetBgmVolume()
    {
        return _audioController.GetBgmVolume();
    }
    public void SetBgmVolume(float volume)
    {
        _audioController.SetBgmVolume(volume);
    }
    public float GetSFXVolume()
    {
        return _audioController.GetSFXVolume();
    }

    public void SetSFXVolume(float volume)
    {
        _audioController.SetSFXVolume(volume);
    }

    public async UniTask LoadAudioClipsAsync()
    {
        if (!DataManager.Instance.TryGetTable(out Dictionary<string, AudioData> audioDataTable))
        {
            Debug.LogError($"[{nameof(AudioManager)}] 오디오 데이터 테이블을 가져오지 못했습니다.");
            return;
        }

        foreach (KeyValuePair<string, AudioData> audioRecord in audioDataTable)
        {
            string audioClipId = audioRecord.Key;
            string assetAddress = audioRecord.Value.AudioClipAddress;

            AudioClip audioClip = await ResourceManager.Instance.GetAssetAsync<AudioClip>(assetAddress);

            if (audioClip == null)
            {
                Debug.LogError($"[{nameof(AudioManager)}] LoadAudioClipsAsync에서 오디오 에셋 로드에 실패했습니다. (Address: {assetAddress})");
                continue;
            }

            if (_cachedAudioClips.ContainsKey(audioClipId))
            {
                Debug.LogWarning($"[{nameof(AudioManager)}] 중복된 오디오 ID가 존재하여 기존 데이터를 덮어씁니다.");
            }

            _cachedAudioClips[audioClipId] = audioClip;
        }
    } 
}