using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HomingProjectile : SkillObject
{
    private CancellationTokenSource _cancellationTokenSource;
     
    private Vector3 _moveDirection;
    private float _chainSearchRadius;
    private float _speed;
    private float _rotateSpeed;
    
    private Transform _target;

    private ContactFilter2D _contactFilter;
    private readonly List<Collider2D> _targetList = new List<Collider2D>(128);

    private void Update()
    {
        if (_target == null || !_target.gameObject.activeInHierarchy)
        {
            _target = FindNearestTarget();

            if (_target == null)
            {
                transform.position += _speed * Time.deltaTime * _moveDirection; 
                return;
            }
        }

        Vector2 direction = (Vector2)_target.position - (Vector2)transform.position;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);

        transform.Translate(_speed * Time.deltaTime * Vector2.right, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string attackId = null;

        if (_ownerType == UnitType.Player && collision.CompareTag(GameTags.Enemy))
        {
            attackId = _dataId;
        }
        else if (_ownerType == UnitType.Enemy && collision.CompareTag(GameTags.Player))
        {
            attackId = _ownerId;
        }

        if (attackId == null) { return; }

        if (!collision.TryGetComponent(out BaseStatus targetStatus))
        {
            Debug.LogWarning($"[{gameObject.name}] {collision.name}에 BaseStatus 컴포넌트가 없어 데미지를 주지 못했습니다.");
        }

        targetStatus.TakeDamage(attackId, _damage);
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

    public void Setup(UnitType ownerType, string ownerId, string dataId, Vector3 moveDirection, float damage, float chainSearchRadius, float speed, float rotateSpeed, float duration, Transform startTarget)
    {
        Setup(ownerType, ownerId, dataId, damage);

        _moveDirection = moveDirection;
        _chainSearchRadius = chainSearchRadius;
        _speed = speed;
        _rotateSpeed = rotateSpeed;
        _target = startTarget;

        float angle = (Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        switch (ownerType)
        {
            case UnitType.Player:
                _contactFilter.SetLayerMask(LayerMask.GetMask(GameLayers.Enemy));
                break;
            case UnitType.Enemy:
                _contactFilter.SetLayerMask(LayerMask.GetMask(GameLayers.Player));
                break;
        }

        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        _cancellationTokenSource = new CancellationTokenSource();

        UniTaskUtils.DelayActionAsync(duration, DespawnHoming, _cancellationTokenSource.Token).Forget();
    }

    private Transform FindNearestTarget()
    {
        _targetList.Clear();

        Vector3 searchPosition = transform.position;

        int targetCount = Physics2D.OverlapCircle(searchPosition, _chainSearchRadius , _contactFilter, _targetList);

        if (targetCount == 0)
        {
            return null;
        }

        Transform nearestTarget = null;

        float minSqrDistance = Mathf.Infinity;

        for (int i = 0; i < targetCount; i++)
        {
            Collider2D target = _targetList[i];

            if (target == null) { continue; }

            if (!target.TryGetComponent(out BaseStatus _))
            {
                Debug.LogWarning($"[{target.name}] BaseStatus 컴포넌트가 없어 제외되었습니다.");
                continue;
            }

            float sqrDistance = (target.transform.position - searchPosition).sqrMagnitude;

            if (sqrDistance < minSqrDistance)
            {
                minSqrDistance = sqrDistance;
                nearestTarget = target.transform;
            }
        }

        return nearestTarget;
    }

    private void DespawnHoming()
    {
        ObjectManager.Instance.DespawnObject(InstanceId);
    }
}