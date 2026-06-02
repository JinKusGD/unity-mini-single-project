using UnityEngine;
using UnityEngine.Events;

public class UIBase : MonoBehaviour
{
    protected void BindButtonEventWithNullCheck(UIButton button, UnityAction action)
    {
        if (button == null) { return; }

        button.BindOnClickEvent(action);
    }
}
