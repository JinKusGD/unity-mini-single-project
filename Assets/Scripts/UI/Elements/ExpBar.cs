using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Text level;

    private void OnEnable()
    {
        EventBus.Subscribe<ExpInfo>(OnExpChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ExpInfo>(OnExpChanged);
    }

    private void OnExpChanged(ExpInfo expInfo)
    {
        image.fillAmount = (float)(expInfo.CurrentExp/expInfo.RequiredExp);
        level.text = expInfo.Level.ToString();
    }
}
