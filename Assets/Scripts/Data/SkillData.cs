using System;

[Serializable]
public class SkillData : GameData
{
    public string SkillPattern;
    public int Level;
    public string Name;
    public string Description;
    public float BaseDamage;
    public float DamageMultiplier;
    public float Cooldown;
    public int Count;
    public float Delay;
    public string SpriteId;
    public string SkillAddress;
    public string NextLevelId;
    public string ProjectileSkillId;
    public string OrbitingSkillId;
    public string HomingSkillId;
    public string ArcZoneSkillId;
    public string MeleeSkillId;
    public string RandomTargetSkillId;
    public string AreaSkillId;
}