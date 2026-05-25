using UnityEngine;

public class TitleUI : UIBase
{
    [SerializeField] private UIButton _templateButton;

    public void OnEnable()
    {
        BindButtonEventWithNullCheck(_templateButton, TemplateMethod);
    }

    private void TemplateMethod()
    {

    }
}
