using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public static class CombatUtils
{
    public static float CalculateDamage(float baseDamage, float ownerPower, float damageMultiplier)
    {
        float calculateDamage = (baseDamage + ownerPower) * damageMultiplier;

        return calculateDamage;
    }
}

public static class UniTaskUtils
{
    public static async UniTask DelayAsync(float duration, CancellationToken token)
    {
        try
        {
            TimeSpan delayTime = TimeSpan.FromSeconds(duration);
            await UniTask.Delay(delayTime, cancellationToken: token);
        }
        catch (OperationCanceledException) { }
    }

    public static async UniTask DelayActionAsync(float duration, Action onComplete, CancellationToken token)
    {
        if (onComplete == null) { return; }

        try
        {
            TimeSpan delayTime = TimeSpan.FromSeconds(duration);

            await UniTask.Delay(delayTime, cancellationToken: token);

            onComplete.Invoke();
        }
        catch (OperationCanceledException) { }
    }
}