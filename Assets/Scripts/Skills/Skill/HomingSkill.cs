using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public abstract class HomingSkill : BaseSkill
{
    [SerializeField] protected string _homingProjectileId;
    [SerializeField] protected string _homingProjectileAddress;
    [SerializeField] protected float _searchRadius;
    [SerializeField] protected float _chainSearchRadius;
    [SerializeField] protected float _speed;
    [SerializeField] protected float _rotateSpeed;
    [SerializeField] protected float _duration;

    protected bool _isInitialized;
    protected ContactFilter2D _contactFilter;
    protected readonly List<Collider2D> _targetList = new List<Collider2D>(128);

    protected void InitSkillData(string skillId)
    {
        if (!DataManager.Instance.TryGetData(skillId, out SkillData skillData))
        {
            Debug.LogError($"[{skillId}] 스킬 데이터가 없습니다.");
            return;
        }

        if (skillData.HomingSkillId == null)
        {
            Debug.LogError($"[{skillData.Id}] 유도 스킬 Id가 없습니다.");
            return;
        }

        if (!DataManager.Instance.TryGetData(skillData.HomingSkillId, out HomingSkillData homingSkillData))
        {
            Debug.LogError($"[{skillData.HomingSkillId}] 유도 스킬 데이터가 없습니다.");
            return;
        }

        base.Init(skillData);

        _homingProjectileId = homingSkillData.Id;
        _homingProjectileAddress = homingSkillData.HomingProjectileAddress;
        _searchRadius = homingSkillData.SearchRadius;
        _chainSearchRadius = homingSkillData.ChainSearchRadius;
        _speed = homingSkillData.Speed;
        _rotateSpeed = homingSkillData.RotateSpeed;
        _duration = homingSkillData.Duration;

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

        _isInitialized = true;
    }

    protected override void Fire()
    {
        if (!_isInitialized)
        {
            Debug.LogError($"[{gameObject.name}] 스킬이 초기화 되지 않았습니다.");
            return;
        }

        SpawnHomingProjectile().Forget();
    }

    protected async UniTask SpawnHomingProjectile()
    {
        for (int count = 0; count < _count; count++)
        {
            if (_ownerStatus == null)
            {
                Debug.LogWarning($"[{_homingProjectileId}] 연사 도중 시전자가 소멸하여 남은 스킬 오브젝트 생성을 중단합니다.");
                return;
            }

            GameObject homingProjectile = await ObjectManager.Instance.SpawnSkillObjectAsync(_dataId, _homingProjectileAddress, _ownerStatus.transform.position);

            if (homingProjectile == null)
            {
                Debug.LogError($"[{_homingProjectileId}] 투사체를 생성하지 못했습니다.");
                return;
            }

            if (!homingProjectile.TryGetComponent(out HomingProjectile homingProjectileComponent))
            {
                Debug.LogError($"[{homingProjectile.name}] 생성된 투사체에 HomingProjectile 컴포넌트가 없습니다.");
                ObjectManager.Instance.DestroyObject(homingProjectile);
                return;
            }

            if (_ownerStatus == null)
            {
                Debug.LogWarning($"[{homingProjectile.name}] 시전자가 소멸하여 스킬이 디스폰 되었습니다.");
                ObjectManager.Instance.DespawnObject(homingProjectile);
                return;
            }

            if (!_ownerStatus.TryGetComponent(out SpriteRenderer ownerSpriteRenderer))
            {
                Debug.LogError($"[{_ownerStatus}] 유도 스킬을 생성하기 위한 SpriteRenderer 컴포넌트가 없습니다.");
                ObjectManager.Instance.DestroyObject(homingProjectile);
                return;
            }

            FindTarget();

            Transform startTarget = null;

            if (_targetList.Count > 0)
            {
                int randomIndex = Random.Range(0, _targetList.Count);
                startTarget = _targetList[randomIndex].transform;
            }

            Vector3 moveDirection = ownerSpriteRenderer.flipX ? Vector3.left : Vector3.right;
            float damage = CombatUtils.CalculateDamage(_baseDamage, _ownerStatus.Power, _damageMultiplier);
            homingProjectileComponent.Setup(_ownerStatus.UnitType, _ownerStatus.DataId, _dataId, moveDirection, damage, _chainSearchRadius, _speed, _rotateSpeed, _duration, startTarget);

            await UniTaskUtils.DelayAsync(_delay, this.GetCancellationTokenOnDestroy());
        }
    }

    protected void FindTarget()
    {
        _targetList.Clear();

        Physics2D.OverlapCircle(_ownerStatus.transform.position, _searchRadius, _contactFilter, _targetList);
    }
}