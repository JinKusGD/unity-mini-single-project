using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class MeleeSkill : BaseSkill
{
    [SerializeField] protected string _meleeId;
    [SerializeField] protected string _meleeAddress;
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

        if (skillData.MeleeSkillId == null)
        {
            Debug.LogError($"[{skillData.Id}] 근접 스킬 Id가 없습니다.");
            return;
        }

        if (!DataManager.Instance.TryGetData(skillData.MeleeSkillId, out MeleeSkillData meleeSkillData))
        {
            Debug.LogError($"[{skillData.MeleeSkillId}] 근접 스킬 데이터가 없습니다.");
            return;
        }

        base.Init(skillData);
        
        _meleeId = meleeSkillData.Id;
        _meleeAddress = meleeSkillData.MeleeAddress;
        _duration = meleeSkillData.Duration;
        _scale = new Vector3(meleeSkillData.ScaleX, meleeSkillData.ScaleY, 1);
        _isInitialized = true;
    }

    protected override void Fire()
    {
        if (!_isInitialized)
        {
            Debug.LogError($"[{gameObject.name}] 스킬이 초기화 되지 않았습니다.");
            return;
        }

        SpawnMelee().Forget();
    }

    protected async UniTask SpawnMelee()
    {
        for (int count = 0; count < _count; count++)
        {
            if (_ownerStatus == null)
            {
                Debug.LogWarning($"[{_meleeId}] 연사 도중 시전자가 소멸하여 남은 근접 스킬 생성을 중단합니다.");
                return;
            }

            if (!_ownerStatus.TryGetComponent(out SpriteRenderer ownerSpriteRenderer))
            {
                Debug.LogError($"[{_ownerStatus}] 근접 스킬을 생성하기 위한 SpriteRenderer 컴포넌트가 없습니다.");
                return;
            }

            Vector3 lookDirection = ownerSpriteRenderer.flipX ? Vector3.left : Vector3.right;
            Vector3 spawnPosition = _ownerStatus.transform.position + new Vector3((_scale.y + 0.5f) * lookDirection.x, 0, 0);

            GameObject melee = await ObjectManager.Instance.SpawnSkillObjectAsync(_dataId, _meleeAddress, spawnPosition);
            melee.transform.localScale = _scale;

            if (melee == null)
            {
                Debug.LogError($"[{_meleeId}] 근접 스킬을 생성하지 못했습니다.");
                return;
            }

            if (_ownerStatus == null)
            {
                Debug.LogWarning($"[{melee.name}] 시전자가 소멸하여 스킬이 디스폰 되었습니다.");
                ObjectManager.Instance.DespawnObject(melee);
                return;
            }
   
            if (!melee.TryGetComponent(out Melee meleeComponent))
            {
                Debug.LogError($"[{melee.name}] 생성된 근접 스킬에 Melee 컴포넌트가 없습니다.");
                ObjectManager.Instance.DespawnObject(melee);
                return;
            }

            float damage = CombatUtils.CalculateDamage(_baseDamage, _ownerStatus.Power, _damageMultiplier);
            Debug.Log(damage);
            meleeComponent.Setup(_ownerStatus.UnitType, damage, _duration, lookDirection);

            await UniTaskUtils.DelayAsync(_delay, this.GetCancellationTokenOnDestroy());
        }
    }
}