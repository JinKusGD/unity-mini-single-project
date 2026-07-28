using UnityEngine;

public class OptionUI : UIBase
{
    [SerializeField] private ButtonBase closeAlseButton;

    private void OnEnable()
    {
        BindButtonEventWithNullCheck(closeAlseButton, CloseUI);
    }

    private void CloseUI()
    {
        UIManager.Instance.CloseOptionUI();
    }
}
