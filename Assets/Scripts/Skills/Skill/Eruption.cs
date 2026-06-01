using UnityEngine;

public class Eruption : RandomTargetSkill
{
    private int _currentLevel = 1;

    private void Start()
    {
        string skillDataId = $"Skill_006_Eruption_Lv01";
        InitSkillData(skillDataId);
    }

    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LevelUp();
        }

        base.Update();
    }


    public void LevelUp()
    {
        _currentLevel++;
        string skillDataId = $"Skill_006_Eruption_Lv{_currentLevel:D2}";
        InitSkillData(skillDataId);
    }
}
