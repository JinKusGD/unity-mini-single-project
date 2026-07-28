using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyStatus : BaseStatus
{
    private int _expValue;

    public float AttackCooldown { get; private set; } = 0.5f;

    public override UnitType UnitType
    {
        get { return UnitType.Enemy; }
    }

    public override void TakeDamage(string skillId, float damage)
    {
        if (Hp <= 0) { return; }

        base.TakeDamage(skillId, damage);
        UIManager.Instance.ShowDamagePopupText(damage, transform.position, Color.yellow);
        EventBus.Invoke(new WeaponDamageInfo(skillId, damage));
    }

    protected override void Die()
    {
        ObjectManager.Instance.DespawnObject(gameObject);
        DropExpCore().Forget();
    }

    public void InitStatus(EnemyData enemyData, float power)
    {
        base.InitStatus(enemyData.Id, enemyData.MaxHp * power, enemyData.AttackPower * power, enemyData.MoveSpeed * power);
        _expValue = enemyData.ExpValue;
    }

    private async UniTask DropExpCore()
    {
        GameObject expCore = await ObjectManager.Instance.SpawnExpCoreAsync(transform.position);

        if (!expCore.TryGetComponent(out ExpCore expCoreComponent))
        {
            Debug.LogError($"[{expCore}] ExpCore 컴포넌트가 없습니다.");
        }

        expCoreComponent.SetupExpValue(_expValue);
    }
}