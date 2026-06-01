using System;

[Serializable]
public class ProjectileSkillData : GameData
{
    public string ProjectileAddress;
    public float SearchRadius;
    public float Speed;
    public int MaxHits;
    public float Duration;
    public float Scale;
}