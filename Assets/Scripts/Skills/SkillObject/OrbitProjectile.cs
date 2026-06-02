using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.U2D;

public class OrbitProjectile : SkillObject
{
    private CancellationTokenSource _cancellationTokenSource;

    private Transform _ownerTransform;
    private float _radius;
    private float _rotateSpeed;
    private float _angle;
   
    private void Start()
    {
        if (_ownerTransform == null) return;

        Vector3 offset = transform.position - _ownerTransform.position;

        _angle = Mathf.Atan2(offset.y, offset.x);
    }

    void Update()
    {
        if (_ownerTransform == null) return;

        _angle += _rotateSpeed * Time.deltaTime;

        float x = Mathf.Cos(_angle) * _radius;
        float y = Mathf.Sin(_angle) * _radius;
        transform.position = _ownerTransform.position + new Vector3(x, y, 0f);
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

        targetStatus.TakeDamage(_dataId, _damage);
    }

    public void Setup(Transform ownerTransform, UnitType ownerType, string ownerId, string dataId, float damage, float radius, float rotateSpeed, float startAngle, float duration, Vector3 scale)
    {
        Setup(ownerType, ownerId, dataId, damage);

        _ownerTransform = ownerTransform;
        _radius = radius;
        _rotateSpeed = rotateSpeed;
        _angle = startAngle;

        transform.localScale = scale;

        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        _cancellationTokenSource = new CancellationTokenSource();

        UniTaskUtils.DelayActionAsync(duration, DespawnOrbitProjectile, _cancellationTokenSource.Token).Forget();
    }

    private void DespawnOrbitProjectile()
    {
        ObjectManager.Instance.DespawnObject(InstanceId);
    }
}