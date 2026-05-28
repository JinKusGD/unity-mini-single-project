using UnityEngine;

public class EnemyStatus : BaseStatus
{
    public override UnitType UnitType
    {
        get { return UnitType.Enemy; }
    }

    protected override void Die()
    {
        ObjectManager.Instance.DespawnObject(InstanceId);
    }
}
