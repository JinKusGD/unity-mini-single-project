public class HeatSeeker : HomingSkill
{
    private int _currentLevel = 1;

    private void Start()
    {
        string skillDataId = $"Skill_003_HeatSeeker_Lv01";
        InitSkillData(skillDataId);
    }

    public void LevelUp()
    {
        _currentLevel++;
        string skillDataId = $"Skill_003_HeatSeeker_Lv{_currentLevel:D2}";
        InitSkillData(skillDataId);
    }
}
