using UnityEngine;

public class CollectionButton : ButtonBase
{
    public void ChangeImage(Sprite sprite)
    {
        _buttonImage.sprite = sprite;
    }

    public void ChangeText(string text)
    {
        _buttonTexT.text = text;
    }

    protected override void OnClickDefaultEvent()
    {

    }
}
