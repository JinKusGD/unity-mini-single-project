using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class RandomIndan : MonoBehaviour, IPoolableObject
{
    public int InstanceId { get; private set; }

    public bool IsActive { get; private set; }

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

    public void Init(float duration, float areaSize)
    {
        transform.localScale = Vector3.one * areaSize;
        DespawnAfterDuration(duration).Forget();
    }

    private async UniTask DespawnAfterDuration(float duration)
    {
        TimeSpan delayMs = TimeSpan.FromSeconds(duration);
        try
        {
            await UniTask.Delay(delayMs);

            ObjectManager.Instance.DespawnObject(InstanceId);
        }
        catch (OperationCanceledException)
        {
            // Ignore cancellation
        }
    }

    public void Initialize(int instanceId)
    {
        throw new NotImplementedException();
    }
}
