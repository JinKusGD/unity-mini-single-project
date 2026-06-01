using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class OrbitProjectileSkill : BaseSkill
{
    [SerializeField] protected string _orbitProjectileId;
    [SerializeField] protected string _orbitProjectileAddress;
    [SerializeField] protected float _radius;
    [SerializeField] protected float _rotateSpeed;
    [SerializeField] protected float _duration;
    [SerializeField] protected Vector3 _scale;

    protected bool _isInitialized;

    protected void InitSkillData(string skillId)
    {
        if (!DataManager.Instance.TryGetData(skillId, out SkillData skillData))
        {
            Debug.LogError($"[{skillId}] 스킬 데이터가 없습니다.");
            return;
        }

        if (skillData.OrbitingSkillId == null)
        {
            Debug.LogError($"[{skillData.Id}] 공전 스킬 Id가 없습니다.");
            return;
        }

        if (!DataManager.Instance.TryGetData(skillData.OrbitingSkillId, out OrbitingSkillData orbitingSkillData))
        {
            Debug.LogError($"[{skillData.OrbitingSkillId}] 공전 스킬 데이터가 없습니다.");
            return;
        }

        base.Init(skillData);

        _orbitProjectileId = orbitingSkillData.Id;
        _orbitProjectileAddress = orbitingSkillData.OrbitingProjectileAddress;
        _radius = orbitingSkillData.Radius;
        _rotateSpeed = orbitingSkillData.RotateSpeed;
        _duration = orbitingSkillData.Duration;
        _scale = new Vector3(orbitingSkillData.Scale, orbitingSkillData.Scale, 1);

        _isInitialized = true;
    }

    protected override void Fire()
    {
        if (!_isInitialized)
        {
            Debug.LogError($"[{gameObject.name}] 스킬이 초기화 되지 않았습니다.");
            return;
        }

        SpawnOrbitingProjectile().Forget();
    }

    protected async UniTask SpawnOrbitingProjectile()
    {
        for (int i = 0; i < _count; i++)
        {
            if (_ownerStatus == null)
            {
                Debug.LogWarning($"[{_orbitProjectileId}] 연사 도중 시전자가 소멸하여 남은 투사체 생성을 중단합니다.");
                return;
            }

            float startAngle = (i * Mathf.PI * 2f) / _count;

            float spawnX = Mathf.Cos(startAngle) * _radius;
            float spawnY = Mathf.Sin(startAngle) * _radius;

            Vector3 spawnPosition = _ownerStatus.transform.position + new Vector3(spawnX, spawnY, 0f);

            GameObject orbitProjectile = await ObjectManager.Instance.SpawnSkillObjectAsync(_orbitProjectileId, _orbitProjectileAddress, spawnPosition);

            if (orbitProjectile == null)
            {
                Debug.LogError($"[{_orbitProjectileId}] 투사체를 생성하지 못했습니다.");
                return;
            }

            if (_ownerStatus == null)
            {
                Debug.LogWarning($"[{orbitProjectile.name}] 시전자가 소멸하여 스킬이 디스폰 되었습니다.");
                ObjectManager.Instance.DespawnObject(orbitProjectile);
                return;
            }

            if (!orbitProjectile.TryGetComponent(out OrbitProjectile orbitProjectileComponent))
            {
                Debug.LogError($"[{orbitProjectile.name}] 생성된 투사체에 OrbitProjectile 컴포넌트가 없습니다.");
                ObjectManager.Instance.DespawnObject(orbitProjectile);
                return;
            }

            float damage = CombatUtils.CalculateDamage(_baseDamage, _ownerStatus.Power, _damageMultiplier);
            orbitProjectileComponent.Setup(_ownerStatus.transform, _ownerStatus.UnitType, damage, _radius, _rotateSpeed, startAngle, _duration, _scale);

            await UniTaskUtils.DelayAsync(_delay, this.GetCancellationTokenOnDestroy());
        }
    }
}