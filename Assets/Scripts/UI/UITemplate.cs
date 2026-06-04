using UnityEngine;

public class UITemplate : UIBase
{
    [SerializeField] private ButtonBase _templateButton;

    public void OnEnable()
    {
        BindButtonEventWithNullCheck(_templateButton, TemplateMethod);
    }

    private void TemplateMethod()
    {

    }
}
