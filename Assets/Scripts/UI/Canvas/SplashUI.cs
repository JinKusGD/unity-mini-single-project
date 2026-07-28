using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class SplashUI : UIBase
{
    [SerializeField] private LogoImage _logoImage;

    private CancellationTokenSource _cancellationToken;

    private void Start()
    {
        PlaySplashSequenceAsync().Forget();
    }

    private void OnDisable()
    {
        UniTaskUtils.ClearToken(ref _cancellationToken);
    }

    private async UniTask PlaySplashSequenceAsync()
    {
        _cancellationToken = new CancellationTokenSource();
        //_cancellationToken(ref _cancellationToken);

        _logoImage.PlayAnimation();

        await DataManager.Instance.PreloadDataAsync();
        await AudioManager.Instance.LoadAudioClipsAsync();
        AudioManager.Instance.PlaySFX("AudioClip_001_Splash");

        await DataManager.Instance.LoadMainDataAsync();
        await ResourceManager.Instance.LoadSpriteAsync();
        await UniTask.WaitUntil(CheckAnimationFinished, cancellationToken: _cancellationToken.Token);
        await UniTask.Delay(1500);

        await UIManager.Instance.OpenTitleUIAsync();
        UIManager.Instance.CloseSplashUI();


       await ResourceManager.Instance.LoadSpriteAsync();



    }

    private bool CheckAnimationFinished()
    {
        bool result = _logoImage.IsFinished;
        return result;
    }
}