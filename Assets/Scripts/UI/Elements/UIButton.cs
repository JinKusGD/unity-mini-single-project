using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _buttonImage;
    [SerializeField] private Text _buttonTexT;


    private void OnEnable()
    {
        BindOnClickEvent(OnClickDefaultEvent);
    }

    private void OnDisable()
    {
        UnBindOnClickEvent();
    }
    
    public void EnableButton()
    {
        if (_button == null || _buttonImage == null) return;

        _button.interactable = true;
        _buttonImage.color = Color.white;
    }

    public void DisableButton()
    {
        if (_button == null || _buttonImage == null) return;

        _button.interactable = false;
        _buttonImage.color = Color.gray;
    }

    public void BindOnClickEvent(UnityAction onClick)
    {
        if (_button == null) { return; }

        _button.onClick.AddListener(onClick);
    }

    private void UnBindOnClickEvent()
    {
        if (_button == null) { return; }

        _button.onClick.RemoveAllListeners();
    }

    private void OnClickDefaultEvent()
    {
        //클릭 사운드 추후 추가
    }
}
