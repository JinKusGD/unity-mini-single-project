public class Pyroblast : ProjectileSkill
{
    private int _currentLevel = 1;

    private void Start()
    {
        string skillDataId = $"Skill_001_Pyroblast_Lv01";
        InitSkillData(skillDataId);
    }

    public void LevelUp()
    {
        _currentLevel++;
        string skillDataId = $"Skill_001_Pyroblast_Lv{_currentLevel:D2}";
        InitSkillData(skillDataId);
    }
}
