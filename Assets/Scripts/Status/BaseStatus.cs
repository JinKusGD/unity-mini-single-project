using UnityEngine;

public abstract class BaseStatus : MonoBehaviour
{
    public string DataId { get; private set; }

    public abstract UnitType UnitType { get; }

    public float MaxHp { get; protected set; }

    public float Hp { get; protected set; }

    public float Power { get; protected set; }

    public float MoveSpeed { get; protected set; }

    public virtual void InitStatus(string dataId, float maxHp, float power, float moveSpeed)
    {
        DataId = dataId;
        MaxHp = maxHp;
        Hp = maxHp;
        Power = power;
        MoveSpeed = Mathf.Clamp(moveSpeed, 0f, 5f);
    }

    public virtual void TakeDamage(string id, float damage)
    {
        Hp -= damage;

        if (Hp <= 0)
        {
            Hp = 0;
            Die();
        }
    }

    protected abstract void Die();
}