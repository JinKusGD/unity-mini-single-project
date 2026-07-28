public interface ISpawnableObject
{
    int InstanceId { get; }

    void Initialize(int instanceId);
}

public interface IPoolableObject : ISpawnableObject
{
    bool IsActive { get; }
}

public interface ILevelable
{
    void LevelUp(string nextLevelId);
}
