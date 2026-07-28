using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class LevelUpPopupSlot : UIBase
{
    [SerializeField] private Image _slotIconImage;
    [SerializeField] private Text _levelText;
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _descriptionText;
    [SerializeField] private LevelUpButton _levelupButton;

    private SkillData _skillData;

    public void OnEnable()
    {
        BindButtonEventWithNullCheck(_levelupButton, OnSelectSlot);
    }

    public void SetupSlot(string dataId)
    {
        if(!DataManager.Instance.TryGetData(dataId, out _skillData))
        {
            Debug.LogError($"[레벨업 UI] {dataId} 스킬 데이터가 없습니다.");
        }

        _slotIconImage.sprite = ResourceManager.Instance.LoadSprite(_skillData.SpriteId);
        _levelText.text = $"Lv.{_skillData.Level}";
        _nameText.text = _skillData.Name;
        _descriptionText.text = _skillData.Description.Replace("\\n", "\n");
    }

    private void OnSelectSlot()
    {
        SelectSkillAsync().Forget();
    }

    private async UniTask SelectSkillAsync()
    {
        ILevelable levelable = await SkillManager.Instance.GetSkillObject(_skillData.SkillAddress);

        if(levelable != null)
        {
            levelable.LevelUp(_skillData.Id);
        }

        UIManager.Instance.CloseLevelUpUI();
    }
}
