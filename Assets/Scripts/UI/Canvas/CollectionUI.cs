using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionUI : UIBase
{
    [SerializeField] private GameObject SlotPrefab;
    [SerializeField] private Transform ScrollViewContentTransform;
    [SerializeField] private ButtonBase closeAlseButton;
 
    private Dictionary<string, CollectionSlot> _slotDict = new Dictionary<string, CollectionSlot>();
    
    private string _selectedSlotId;

    private void Awake()
    {
        CreateSlot().Forget();

    }

    private void OnEnable()
    {
        BindButtonEventWithNullCheck(closeAlseButton, CloseUI);
    }

    private void CloseUI()
    {
        UIManager.Instance.CloseCollectionUI();
    }

    private async UniTask CreateSlot()
    {
        DataManager.Instance.TryGetTable(out Dictionary<string, EnemyData> targetTable);

        foreach(var a in targetTable)
        {
            CollectionSlot slotObject = await ObjectManager.Instance.SpawnMonsterSlotAsync(ScrollViewContentTransform);

            await slotObject.InitSlot(a.Key, a.Value.Name);

            _slotDict.Add(a.Key, slotObject);
            slotObject.BindCollectionUIAction(OnSlotClicked);
        }
    }

    private void OnSlotClicked(string id)
    {
        if (!_slotDict.TryGetValue(id, out CollectionSlot slotComponent)) 
        {
            return;
        }

        _selectedSlotId = id;
    }
}
