using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class PlayerStatus : BaseStatus
{
    public int Level;
    public int CurrentExp;
    public int RequiredExp;

    private void OnEnable()
    {
        Level = 0;
        CurrentExp = 0;

        LevelUp();
        EventBus.Invoke(new ExpInfo(Level, CurrentExp, RequiredExp));
    }

    public override UnitType UnitType
    {
        get { return UnitType.Player; }
    }

    public override void InitStatus(string dataId, float maxHp, float power, float moveSpeed)
    {
        base.InitStatus(dataId, maxHp, power, moveSpeed);
        EventBus.Invoke(new PlayerHpInfo(Hp, MaxHp));
    }

    protected override void Die()
    {
        ObjectManager.Instance.DespawnObject(gameObject);
        UIManager.Instance.OpenResultAsync().Forget();
    }

    public override void TakeDamage(string skillId, float damage)
    {
        if (Hp <= 0) { return; }

        base.TakeDamage(skillId, damage);

        UIManager.Instance.ShowDamagePopupText(damage, transform.position, Color.red);
        EventBus.Invoke(new PlayerHpInfo(Hp, MaxHp));
    }

    public void AddExp(int exp)
    {
        CurrentExp += exp;

        if (CurrentExp >= RequiredExp)
        {
            LevelUp();
        }

        EventBus.Invoke(new ExpInfo(Level, CurrentExp, RequiredExp));
    }

    private void LevelUp()
    {
        Level++;
        MaxHp += 5;
        Hp = MaxHp;
        Power += 2;
        MoveSpeed += 0.5f;

        CurrentExp -= RequiredExp;

        int TempExt = GetRequiredExp();
        RequiredExp = (TempExt - RequiredExp);

        if (!SkillManager.Instance.HasNextSkill())
        {
            return;
        }
        EventBus.Invoke(new PlayerHpInfo(Hp, MaxHp));
        UIManager.Instance.OpenLevelUpUIAsync().Forget();
    }

    private int GetRequiredExp()
    {
        int RequiredExp;

        string LevelId = $"Level_{Level:D3}";

        if (!DataManager.Instance.TryGetData(LevelId, out ExpData expData))
        {
            Debug.LogError($"[{LevelId}] 요구 경험치 데이터가 없습니다.");
        }

        RequiredExp = expData.RequiredExp;

        return RequiredExp;
    }
}
