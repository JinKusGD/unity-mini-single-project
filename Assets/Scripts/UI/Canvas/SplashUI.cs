using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class SplashUI : MonoBehaviour
{
    [SerializeField] LogoUI logoUI;

    private CancellationTokenSource _cancellationToken;

    private void Start()
    {
        _cancellationToken = new CancellationTokenSource();
        LoadProcess(_cancellationToken.Token).Forget();
    }

    private void OnDisable()
    {
        if (_cancellationToken == null) { return; }

        _cancellationToken.Cancel();
        _cancellationToken.Dispose();
        _cancellationToken = null;
    }

    private async UniTaskVoid LoadProcess(CancellationToken token)
    {
        logoUI.PlayAnimation();

        await DataManager.Instance.LoadAllDataAsync();
        await AudioManager.Instance.LoadAudioClipsAsync();
        AudioManager.Instance.PlaySFX("AudioClip_001_Splash");

        await UniTask.WaitUntil(CheckAnimationFinished, cancellationToken: token);

        await UIManager.Instance.OpenTitleUI();
        UIManager.Instance.CloseSplashUI();
    }

    private bool CheckAnimationFinished()
    {
        return logoUI.IsFinished;
    }
}
