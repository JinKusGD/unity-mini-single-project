using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class ButtonBase : MonoBehaviour
{
    [SerializeField] protected Button _button;
    [SerializeField] protected Image _buttonImage;
    [SerializeField] protected Text _buttonTexT;

    protected virtual void OnEnable()
    {
        BindOnClickEvent(OnClickDefaultEvent);
    }

    protected virtual void OnDisable()
    {
        UnBindOnClickEvent();
    }
    
    public virtual void EnableButton()
    {
        if (_button == null || _buttonImage == null) return;

        _button.interactable = true;
        _buttonImage.color = Color.white;
    }

    public virtual void DisableButton()
    {
        if (_button == null || _buttonImage == null) return;

        _button.interactable = false;
        _buttonImage.color = Color.gray;
    }

    public virtual void BindOnClickEvent(UnityAction onClick)
    {
        if (_button == null) { return; }

        _button.onClick.AddListener(onClick);
    }

    protected virtual void UnBindOnClickEvent()
    {
        if (_button == null) { return; }

        _button.onClick.RemoveAllListeners();
    }

    protected abstract void OnClickDefaultEvent();
}
