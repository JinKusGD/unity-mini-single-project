using UnityEngine;

public class OrbitalFlame : OrbitProjectileSkill
{
    private int _currentLevel = 1;

    private void Start()
    {
        string skillDataId = $"Skill_002_OrbitalFlame_Lv01";
        InitSkillData(skillDataId);
    }

    public void LevelUp()
    {
        _currentLevel++;
        string skillDataId = $"Skill_002_OrbitalFlame_Lv{_currentLevel:D2}";
        InitSkillData(skillDataId);
    }
}