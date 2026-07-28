using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class FieldPopup : UIBase
{
    [SerializeField] private int delay = 2000;
    [SerializeField] private Text text;

    private CancellationTokenSource _cts;

    private void OnEnable()
    {
        _cts = new CancellationTokenSource();
        CloseAsync().Forget();
    }

    private void OnDisable()
    {
        UniTaskUtils.ClearToken(ref _cts);
    }

    public void ChangeText(string a)
    {
        text.text = a;
    }

    private async UniTask CloseAsync()
    {
        await UniTask.Delay(delay, cancellationToken: _cts.Token);
        UIManager.Instance.CloseFieldPopupHud();
    }
}