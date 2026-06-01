using System;

[Serializable]
public class HomingSkillData : GameData
{
    public string HomingProjectileAddress;
    public float SearchRadius;
    public float ChainSearchRadius;
    public float Speed;
    public float RotateSpeed;
    public float Duration;
}