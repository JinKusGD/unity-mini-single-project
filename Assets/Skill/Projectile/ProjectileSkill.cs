using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSkill : BaseSkill
{
    private string _projectileId;
    private string _projectileAddress;
    private float _searchRadius;
    [SerializeField] private float _speed;
    [SerializeField] private int _maxHits;
    [SerializeField] private float _duration;

    private ContactFilter2D _contactFilter;
    private readonly List<Collider2D> _targetList = new List<Collider2D>(128);

    private void Start()
    {
        Init("Skill_001_Fireball");
    }

    private void Init(string skillId)
    {
        if (!DataManager.Instance.TryGetData(skillId, out SkillData skillData))
        {
            Debug.LogError($"[{skillId}] 스킬 데이터가 없습니다.");
            return;
        }

        base.Init(skillData);

        if (skillData.ProjectileSkillId == null)
        {
            Debug.LogError($"[{skillData.Id}] 투사체 스킬 Id가 없습니다.");
            return;
        }

        if (!DataManager.Instance.TryGetData(skillData.ProjectileSkillId, out ProjectileSkillData projectileSkillData))
        {
            Debug.LogError($"[{skillData.ProjectileSkillId}] 투사체 스킬 데이터가 없습니다.");
            return;
        }

        _projectileId = projectileSkillData.Id;
        _projectileAddress = projectileSkillData.ProjectileAddress;
        _searchRadius = projectileSkillData.SearchRadius;
        _speed = projectileSkillData.Speed;
        _duration = projectileSkillData.Duration;
        _maxHits = projectileSkillData.MaxHits;

        _contactFilter.useTriggers = false;
        _contactFilter.useLayerMask = true;

        switch (_ownerStatus.UnitType)
        {
            case UnitType.Player:
                _contactFilter.SetLayerMask(LayerMask.GetMask(GameLayers.Enemy));
                break;
            case UnitType.Enemy:
                _contactFilter.SetLayerMask(LayerMask.GetMask(GameLayers.Player));
                break;
            default:
                Debug.LogError($"[{_ownerStatus.name}] 스킬을 사용할 수 없는 UnitType 입니다.");
                return;
        }
    }

    protected override void Fire()
    {
        SpawnProjectile().Forget();
    }

    private async UniTask SpawnProjectile()
    {
        for (int count = 0; count < _count; count++)
        {
            if (_ownerStatus == null)
            {
                Debug.LogWarning($"[{_projectileId}] 연사 도중 시전자가 소멸하여 남은 투사체 생성을 중단합니다.");
                return;
            }

            GameObject projectile = await ObjectManager.Instance.SpawnSkillAsync(_projectileId, _projectileAddress, _ownerStatus.transform.position);

            if (projectile == null)
            {
                Debug.LogError($"[{_projectileId}] 투사체를 생성하지 못했습니다.");
                return;
            }

            if (_ownerStatus == null)
            {
                Debug.LogWarning($"[{projectile.name}] 시전자가 소멸하여 스킬이 디스폰 되었습니다.");
                ObjectManager.Instance.DespawnObject(projectile);
                return;
            }

            if (!projectile.TryGetComponent(out Projectile projectileComponent))
            {
                Debug.LogError($"[{projectile.name}] 생성된 투사체에 Projectile 컴포넌트가 없습니다.");
                ObjectManager.Instance.DespawnObject(projectile);
                return;
            }

            Vector3 moveDirection = Vector3.right;

            Transform nearestTarget = FindNearestTarget();

            if (nearestTarget != null)
            {
                Vector3 direction = nearestTarget.position - projectile.transform.position;
                moveDirection = (direction == Vector3.zero) ? moveDirection : direction.normalized;
            }

            float damage = CombatUtils.CalculateDamage(_baseDamage, _ownerStatus.Power, _damageMultiplier);
            Debug.Log(damage);
            projectileComponent.Setup(_ownerStatus.UnitType, damage, moveDirection, _speed, _maxHits, _duration);

            await UniTaskUtils.DelayAsync(_delay, this.GetCancellationTokenOnDestroy());
        }
    }

    private Transform FindNearestTarget()
    {
        _targetList.Clear();

        Vector3 ownerPosition = _ownerStatus.transform.position;

        int targetCount = Physics2D.OverlapCircle(ownerPosition, _searchRadius, _contactFilter, _targetList);

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

            if (!target.TryGetComponent(out BaseStatus targetStatus))
            {
                Debug.LogWarning($"[{target.name}] BaseStatus 컴포넌트가 없어 제외되었습니다.");
                continue;
            }

            float sqrDistance = (target.transform.position - ownerPosition).sqrMagnitude;

            if (sqrDistance < minSqrDistance)
            {
                minSqrDistance = sqrDistance;
                nearestTarget = target.transform;
            }
        }

        return nearestTarget;
    }
}