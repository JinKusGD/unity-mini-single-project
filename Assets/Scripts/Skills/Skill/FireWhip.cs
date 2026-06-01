public class FireWhip : MeleeSkill
{
    private int _currentLevel = 1;

    private void Start()
    {
        string skillDataId = $"Skill_005_FireWhip_Lv01";
        InitSkillData(skillDataId);
    }

    public void LevelUp()
    {
        _currentLevel++;
        string skillDataId = $"Skill_005_FireWhip_Lv{_currentLevel:D2}";
        InitSkillData(skillDataId);
    }
}