using UnityEngine;

public struct MapSize
{
    public float MinX;
    public float MaxX;
    public float MinY;
    public float MaxY;

    public MapSize(float minX, float maxX, float minY, float maxY)
    {
        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
    }
}

public struct PoolResult
{
    public bool IsSuccess { get;  private set; }

    public GameObject ResultObject { get; private set; }

    public PoolResult(bool isSuccess, GameObject resultObject)
    {
        IsSuccess = isSuccess;
        ResultObject = resultObject;
    }
}

public struct WeaponDamageInfo
{
    public string WeaponId;
    public float Damage;

    public WeaponDamageInfo(string id, float damage)
    {
        WeaponId = id;
        Damage = damage;
    }
}

public struct DashCountInfo
{
    public int CurrentCount;
    public int MaxCount;

    public DashCountInfo(int current, int max)
    {
        CurrentCount = current;
        MaxCount = max;
    }
}

public struct PlayerHpInfo
{
    public float CurrentHp;
    public float MaxHp;

    public PlayerHpInfo(float currentHp, float maxHp)
    {
        CurrentHp = currentHp;
        MaxHp = maxHp;
    }
}

public struct ExpInfo
{
    public float Level;
    public float CurrentExp;
    public float RequiredExp;

    public ExpInfo(int level, float currentExp, float requiredExp)
    {
        Level = level;
        CurrentExp = currentExp;
        RequiredExp = requiredExp;
    }
}