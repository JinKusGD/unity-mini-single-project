using UnityEngine;

public class SkillObject : MonoBehaviour, IPoolableObject
{
    protected UnitType _ownerType;
    protected string _ownerId;
    protected string _dataId;
    protected float _damage;

    public int InstanceId { get; private set; }

    public bool IsActive { get; private set; }

    protected virtual void OnEnable()
    {
        IsActive = true;
    }

    protected virtual void OnDisable()
    {
        IsActive = false;
    }

    public void SetInstanceId(int instanceId)
    {
        InstanceId = instanceId;
    }

    protected void Setup(UnitType unitType, string ownerId, string dataId, float damage)
    {
        _ownerType = unitType;
        _ownerId = ownerId;
        _dataId = dataId;
        _damage = damage;
    }
}