using System;

[Serializable]
public abstract class UnitData : GameData
{
    public string Name;
    public string Description;
    public float MaxHp;
    public float AttackPower;
    public float MoveSpeed;
    public string PrefabKey;
}