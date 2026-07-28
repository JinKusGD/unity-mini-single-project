using UnityEngine;
using UnityEngine.UI;

public class HpSlider : MonoBehaviour
{
    [SerializeField] private Image image;

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerHpInfo>(OnPlayerHpChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerHpInfo>(OnPlayerHpChanged);
    }

    private void OnPlayerHpChanged(PlayerHpInfo playerHpInfo)
    {
        image.fillAmount = (float)(playerHpInfo.CurrentHp/playerHpInfo.MaxHp);
    }
}
