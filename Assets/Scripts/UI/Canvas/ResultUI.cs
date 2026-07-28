using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : UIBase
{
    [SerializeField] private Text time;
    [SerializeField] private Text Result;
    [SerializeField] private ButtonBase closeAlseButton;

    private void OnEnable()
    {
        Time.timeScale = 0f;
        BindButtonEventWithNullCheck(closeAlseButton, CloseUI);
        time.text = $"버틴 시간 : {GetPlayTimeFormatted(GameManager.Instance.PlayTime)}";
        Result.text = ResultManager.Instance.PrintDamageRankings();
    }

    private void CloseUI()
    {
        UIManager.Instance.CloseResult();
        Time.timeScale = 1f;
        GameManager.Instance.EndGame().Forget();
    }

    public string GetPlayTimeFormatted(float playTime)
    {
        int hours = (int)(playTime / 3600);
        int minutes = (int)((playTime % 3600) / 60);
        int seconds = (int)(playTime % 60);

        return $"{hours:00}:{minutes:00}:{seconds:00}";
    }
}
