using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public abstract class RandomTargetSkill : BaseSkill
{
    [SerializeField] protected string _randomTargetId;
    [SerializeField] protected string _randomTargetAddress;
    [SerializeField] protected float _searchRadius;
    [SerializeField] protected int _hitCount;
    [SerializeField] protected float _duration;
    [SerializeField] protected Vector3 _scale;

    private bool _isInitialized;

    protected ContactFilter2D _contactFilter;
    protected readonly List<Collider2D> _targetList = new List<Collider2D>(128);

    protected void InitSkillData(string skillId)
    {
        if (!DataManager.Instance.TryGetData(skillId, out SkillData skillData))
        {
            Debug.LogError($"[{skillId}] 랜덤 타겟 스킬 데이터가 없습니다.");
            return;
        }

        if (skillData.RandomTargetSkillId == null)
        {
            Debug.LogError($"[{skillData.Id}] 랜덤 타겟 스킬 Id가 없습니다.");
            return;
        }

        if (!DataManager.Instance.TryGetData(skillData.RandomTargetSkillId, out RandomTargetSkillData randomTargetSkillData))
        {
            Debug.LogError($"[{skillData.RandomTargetSkillId}] 랜덤 타겟 스킬 데이터가 없습니다.");
            return;
        }

        base.Init(skillData);

        _randomTargetId = randomTargetSkillData.Id;
        _randomTargetAddress = randomTargetSkillData.RandomTargetAddress;
        _searchRadius = randomTargetSkillData.SearchRadius;
        _hitCount = randomTargetSkillData.HitCount;
        _duration = randomTargetSkillData.Duration;
        _scale = new Vector3(randomTargetSkillData.ScaleX, randomTargetSkillData.ScaleY, 1);

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

        SpawnRandomTarget().Forget();
    }

    protected async UniTask SpawnRandomTarget()
    {
        for (int count = 0; count < _count; count++)
        {
            if (_ownerStatus == null)
            {
                Debug.LogWarning($"[{_randomTargetId}] 연사 도중 시전자가 소멸하여 남은 스킬 오브젝트 생성을 중단합니다.");
                return;
            }

            FindTarget();

            Vector2 randomOffset = Random.insideUnitCircle * _searchRadius;
            Vector3 spawnPosition = _ownerStatus.transform.position + new Vector3(randomOffset.x, randomOffset.y, 1f);

            if (_targetList.Count > 0)
            {
                int randomIndex = Random.Range(0, _targetList.Count);
                Transform target = _targetList[randomIndex].transform;

                if (target != null)
                {
                    spawnPosition = target.position;
                }
            }

            GameObject randomTarget = await ObjectManager.Instance.SpawnSkillObjectAsync(_dataId, _randomTargetAddress, spawnPosition);

            if (randomTarget == null)
            {
                Debug.LogError($"[{_randomTargetId}] 랜덤 타겟을 생성하지 못했습니다.");
                return;
            }

            if (!randomTarget.TryGetComponent(out RandomTarget randomTargetComponent))
            {
                Debug.LogError($"[{randomTarget.name}] 생성된 랜덤 타겟에 RandomTarget 컴포넌트가 없습니다.");
                ObjectManager.Instance.DestroyObject(randomTarget);
                return;
            }

            if (_ownerStatus == null)
            {
                Debug.LogWarning($"[{randomTarget.name}] 시전자가 소멸하여 스킬이 디스폰 되었습니다.");
                ObjectManager.Instance.DespawnObject(randomTarget);
                return;
            }

            float damage = CombatUtils.CalculateDamage(_baseDamage, _ownerStatus.Power, _damageMultiplier);
            randomTargetComponent.Setup(_ownerStatus.UnitType, _ownerStatus.DataId, _dataId, damage, _hitCount, _duration, _scale);

            await UniTaskUtils.DelayAsync(_delay, this.GetCancellationTokenOnDestroy());
        }
    }

    protected void FindTarget()
    {
        _targetList.Clear();

        Physics2D.OverlapCircle(_ownerStatus.transform.position, _searchRadius, _contactFilter, _targetList);
    }
}