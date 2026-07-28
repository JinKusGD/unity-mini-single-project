using System.Collections.Generic;
using UnityEngine;

public class LevelUpPopup : UIBase
{
    [SerializeField] private LevelUpPopupSlot[] levelUpPopupSlots = new LevelUpPopupSlot[3];

    private void OnEnable()
    {
        List<string> getRandomSkillList = SkillManager.Instance.GetRandomSkillList(levelUpPopupSlots.Length);

        for (int i = 0; i < levelUpPopupSlots.Length; i++)
        {
            if(getRandomSkillList[i] == null)
            {
                levelUpPopupSlots[i].gameObject.SetActive(false);
                continue;
            }

            levelUpPopupSlots[i].SetupSlot(getRandomSkillList[i]);
            levelUpPopupSlots[i].gameObject.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
