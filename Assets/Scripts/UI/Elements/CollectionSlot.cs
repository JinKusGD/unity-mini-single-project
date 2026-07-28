using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class CollectionSlot : MonoBehaviour
{
    [SerializeField] private CollectionButton Button;

    private string _slotId;

    private Action<string> _collectionUIAction;

    private void OnEnable()
    {
        Button.BindOnClickEvent(OnSlotClick);
    }

    private void OnDisable()
    {
        UnBindCollectionUIAction();
    }

    public async UniTask InitSlot(string id, string name)
    {
        _slotId = id;

        Sprite sprite = await ResourceManager.Instance.GetAssetAsync<Sprite>($"{_slotId}[1]");

        Button.ChangeImage(sprite);
        Button.ChangeText(name);
    }

    public void BindCollectionUIAction(Action<string> collectionUICallback)
    {
        _collectionUIAction = collectionUICallback;
    }

    private void UnBindCollectionUIAction()
    {
        _collectionUIAction = null;
    }

    private void OnSlotClick()
    {
        _collectionUIAction?.Invoke(_slotId);

    }
}
