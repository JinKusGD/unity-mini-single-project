using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class Projectile : SkillObject
{
    private CancellationTokenSource _cancellationTokenSource;
    
    private UnitType _ownerType;
    private float _damage;
    private Vector3 _moveDirection;
    private float _speed;
    private int _hitsRemaining;
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
    private void Update()
    {
        transform.position += _speed * Time.deltaTime * _moveDirection;
    }

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
        _hitsRemaining--;

        if (_hitsRemaining <= 0)
        {
            DespawnProjectile();
        }
    }

    public void Setup(UnitType ownerType, float damage, Vector3 moveDirection, float speed, int maxHits, float duration, Vector3 scale)
    {
        _ownerType = ownerType;
        _damage = damage;
        _moveDirection = moveDirection;
        _speed = speed;
        _hitsRemaining = maxHits;

        float angle = (Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.localScale = scale;
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        _cancellationTokenSource = new CancellationTokenSource();

        UniTaskUtils.DelayActionAsync(duration, DespawnProjectile, _cancellationTokenSource.Token).Forget();
    }

    private void DespawnProjectile()
    {
        ObjectManager.Instance.DespawnObject(InstanceId);
    }
}