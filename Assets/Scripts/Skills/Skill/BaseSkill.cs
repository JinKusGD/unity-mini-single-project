using UnityEngine;

public abstract class BaseSkill : MonoBehaviour, ILevelable
{
    protected BaseStatus _ownerStatus;

    [SerializeField] protected string _dataId;
    [SerializeField] protected string _nextLevelId;
    [SerializeField] protected float _baseDamage;
    [SerializeField] protected float _damageMultiplier;
    [SerializeField] protected float _cooldown;
    [SerializeField] protected int _count;
    [SerializeField] protected float _delay;

    private float _cooldownTimer;
    private bool _isActive;

    protected virtual void Awake()
    {
        _ownerStatus = GetComponentInParent<BaseStatus>();

        if (_ownerStatus == null)
        {
            Destroy(gameObject);
            return;
        }
    }

    protected virtual void Update()
    {
        if (!_isActive) { return; }

        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
        }
        
        if(_cooldownTimer <= 0)
        {
            ExecuteSkill();
        }
    }

    public void SetActive(bool isActive)
    {
        _isActive = isActive;

        if (!isActive)
        {
            _cooldownTimer = _cooldown;
        }
    }

    public virtual void LevelUp(string nextLevelId)
    {
        InitSkillData(nextLevelId);
        SkillManager.Instance.AddPoolNextLevelId(_dataId, _nextLevelId);
    }

    protected virtual void Init(SkillData skillData)
    {
        _dataId = skillData.Id;
        _nextLevelId = skillData.NextLevelId;
        _baseDamage = skillData.BaseDamage;
        _damageMultiplier = skillData.DamageMultiplier;
        _cooldown = skillData.Cooldown;
        _count = skillData.Count;
        _delay = skillData.Delay;

        _cooldownTimer = 0f;
        _isActive = true;
    }

    protected virtual void ExecuteSkill()
    {
        Fire();
        _cooldownTimer = _cooldown;
    }

    protected abstract void InitSkillData(string nextLevelId);

    protected abstract void Fire();
}