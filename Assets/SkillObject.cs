using UnityEngine;

public class SkillObject : MonoBehaviour, IPoolableObject
{
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
}
