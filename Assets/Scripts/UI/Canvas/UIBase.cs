using UnityEngine;
using UnityEngine.Events;

public abstract class UIBase : MonoBehaviour
{
    protected virtual void BindButtonEventWithNullCheck(ButtonBase button, UnityAction action)
    {
        if (button == null) { return; }

        button.BindOnClickEvent(action);
    }
}
