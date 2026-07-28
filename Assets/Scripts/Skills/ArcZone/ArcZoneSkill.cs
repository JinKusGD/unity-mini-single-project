//using Cysharp.Threading.Tasks;
//using System.Collections.Generic;
//using UnityEngine;

//public class ArcZoneSkill : BaseSkill
//{
//    [SerializeField] protected string _arcZoneId;
//    [SerializeField] protected string _thrownProjectileAddresss;
//    [SerializeField] protected string _zoneAddress;
//    [SerializeField] protected float _thrownProjectileRadius;
//    [SerializeField] protected float _zoneDuration;
//    [SerializeField] protected float _scale;


//    private LayerMask _targetLayer;
//    private string skillid;
//    private int bottleCount;
//    private object zoneDuration;
//    private object zoneSize;

//    public object ZonePrefabkey { get; private set; }

//    private object _projectilePrefabKey;

//    private void Start()
//    {
//        DataManager.Instance.TryGetData("Skill_004_FireBottle", out SkillData skillData);

//        Init(skillData);
//    }

//    protected override void Fire()
//    {
//        SpawnProjectile().Forget();
//    }

//    protected override void Init(ArcZoneSkill skillData)
//    {
//        base.Init(skillData);
//        skillid = skillData.Id;
//        bottleCount = 3;
//        zoneDuration = skillData.Duration;
//        zoneSize = skillData.ZoneSize;
//        ZonePrefabkey = skillData.ZonePrefabKey;
//        _projectilePrefabKey = skillData.ProjectilePrefabKey;


//        if (_ownerStatus.CompareTag("Player"))
//        {
//            _targetLayer = LayerMask.GetMask("Enemy");
//        }
//        else
//        {
//            _targetLayer = LayerMask.GetMask("Player");
//        }
//    }

//    private async UniTask SpawnProjectile()
//    {

//        for (int i = 0; i < bottleCount; i++)
//        {

//            Vector3 targetPosition = FindTargetPosition();

//            GameObject projectile = await ObjectManager.Instance.SpawnSkillObjectAsync(_dataId, _projectilePrefabKey, _ownerStatus.transform.position);

//            if (projectile == null)
//            {
//                continue;
//            }


//            HolyWaterBottle bottleScript = projectile.GetComponent<HolyWaterBottle>();
//            if (bottleScript != null)
//            {
//                float finalDamage = (_baseDamage + _ownerStatus.Power) * _damageMultiplier;
//                bottleScript.Setup(skillid, targetPosition, finalDamage, zoneDuration, zoneSize, ZonePrefabkey);
//            }
//        }

//    }

//    private Vector3 FindTargetPosition()
//    {
//        Collider2D[] targets = Physics2D.OverlapCircleAll(_ownerStatus.transform.position, spawnRadius, _targetLayer);

//        if (targets.Length > 0)
//        {
//            int randomIndex = Random.Range(0, targets.Length);
//            return targets[randomIndex].transform.position;
//        }
//        else
//        {
//            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
//            return _ownerStatus.transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
//        }
//    }
//}
