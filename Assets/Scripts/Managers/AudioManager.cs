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
            Debug.LogWarning($"[{gameObject.name}] AudioManager 인스턴스가 존재하여 기존 오브젝트를 파괴했습니다.");
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

    public async UniTask LoadAudioClipsAsync()
    {
        if (!DataManager.Instance.TryGetTable(out Dictionary<string, AudioData> audioDataTable))
        {
            Debug.LogError("[오디오 캐싱] 오디오 데이터 테이블을 가져오지 못했습니다.");
            return;
        }

        foreach(KeyValuePair<string, AudioData> audioRecord in audioDataTable)
        {
            if (_cachedAudioClips.ContainsKey(audioRecord.Key))
            {
                Debug.LogWarning($"[오디오 캐싱] : 중복된 오디오 아이디 {audioRecord.Key}가 존재합니다. 기존 데이터를 덮어씁니다.");
            }

            AudioClip audioClip = await ResourceManager.Instance.GetAssetAsync<AudioClip>(audioRecord.Value.AudioClipAddress);

            _cachedAudioClips[audioRecord.Key] = audioClip;
        }
    }
}