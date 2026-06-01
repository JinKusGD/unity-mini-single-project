using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class Melee : SkillObject
{
    private CancellationTokenSource _cancellationTokenSource;

    private UnitType _ownerType;
    private float _damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isTarget = false;

        if (_ownerType == UnitType.Player && collision.CompareTag(GameTags.Enemy))
        {
            isTarget = true;
        }
        else if (_ownerType == UnitType.Enemy && collision.CompareTag(GameTags.Player))
        {
            isTarget = true;
        }

        if (!isTarget) { return; }

        if (!collision.TryGetComponent(out BaseStatus targetStatus))
        {
            Debug.LogWarning($"[{gameObject.name}] {collision.name}에 BaseStatus 컴포넌트가 없어 데미지를 주지 못했습니다.");
        }

        targetStatus.TakeDamage(_damage);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }

    public void Setup(UnitType ownerType, float damage, float duration, Vector2 lookDirection, Vector3 scale)
    {
        _ownerType = ownerType;
        _damage = damage;

        transform.up = lookDirection.normalized;
        transform.localScale = scale;

        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        _cancellationTokenSource = new CancellationTokenSource();

        UniTaskUtils.DelayActionAsync(duration, DespawnMelee, _cancellationTokenSource.Token).Forget();
    }

    private void DespawnMelee()
    {
        ObjectManager.Instance.DespawnObject(InstanceId);
    }
}