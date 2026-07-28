using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private Text text;

    private void LateUpdate()
    {
        float playTime = GameManager.Instance.PlayTime;

        text.text = GetPlayTimeFormatted(playTime);
    }

    public string GetPlayTimeFormatted(float playTime)
    {
        int hours = (int)(playTime / 3600);
        int minutes = (int)((playTime % 3600) / 60);
        int seconds = (int)(playTime % 60);

        return $"{hours:00}:{minutes:00}:{seconds:00}";
    }
}
