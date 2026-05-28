using UnityEngine;

public abstract class BaseStatus : MonoBehaviour, IPoolableObject
{
    public abstract UnitType UnitType { get; }

    public int InstanceId { get; private set; }

    public bool IsActive { get; private set; }

    public float MaxHp { get; private set; }

    public float Hp { get; private set; }

    public float Power { get; private set; }

    public float MoveSpeed { get; private set; }

    private void OnEnable()
    {
        IsActive = true;
    }

    private void OnDisable()
    {
        IsActive = false;
    }

    public void SetInstanceId(int instanceId)
    {
        InstanceId = instanceId;
    }

    public void InitStatus(float maxHp, float power, float moveSpeed)
    {
        MaxHp = maxHp;
        Hp = MaxHp;
        Power = power;
        MoveSpeed = moveSpeed;
    }

    public virtual void TakeDamage(float damage)
    {
        Hp -= damage;

        if (Hp <= 0)
        {
            Die();
        }
    }

    protected abstract void Die();
}