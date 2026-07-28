//using Cysharp.Threading.Tasks;
//using UnityEngine;

//public class RandomZoneWeapon : BaseSkill
//{
//    private BaseStatus _ownerStatus;

//    private string _projectilePrefabKey;
//    public string indicatorPrefab;

//    public int strikeCount;
//    public float delayBetweenStrikes;
//    public float areaSize;
//    public float indicatorDuration;

//    private Camera mainCamera;
//    private void Awake()
//    {
//        if (transform.parent == null)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        if (!transform.parent.TryGetComponent(out _ownerStatus))
//        {
//            Destroy(gameObject);
//            return;
//        }
//    }

//    private void Start()
//    {
//        DataManager.Instance.TryGetData("Skill_006_Metor", out SkillData skillData);

//        Init(skillData);
//    }

//    protected override void Init(SkillData skillData)
//    {
//        base.Init(skillData);

//        _projectilePrefabKey = skillData.ProjectilePrefabKey;
//        indicatorPrefab = skillData.IndicatorPrefabKey;
//        strikeCount = skillData.Count;
//        delayBetweenStrikes = skillData.Delay;
//        areaSize = skillData.ZoneSize;
//        indicatorDuration = skillData.Duration;
//        mainCamera = Camera.main;
//    }

//    protected override void Fire()
//    {
//        BombardmentRoutine().Forget();
//    }

//    private async UniTask BombardmentRoutine()
//    {
//        for (int i = 0; i < strikeCount; i++)
//        {
//            Vector3 targetPosition = GetRandomScreenWorldPosition();

//            if (indicatorPrefab != null)
//            {
//                string indicatorId = $"{_dataId}_Indicator";
//                GameObject indicator = await ObjectManager.Instance.SpawnSkillAsync(indicatorId, indicatorPrefab, targetPosition);

//                indicator.GetComponent<RandomIndan>().Init(indicatorDuration, areaSize);
//            }

//            System.TimeSpan delayMs = System.TimeSpan.FromSeconds(0.1f);
//            await UniTask.Delay(delayMs);

//            GameObject projectile = await ObjectManager.Instance.SpawnSkillAsync(_dataId, _projectilePrefabKey, targetPosition + new Vector3(-3f, 10f, 0f));
//            float finalDamage = (_baseDamage + _ownerStatus.Power) * _damageMultiplier;
//            projectile.GetComponent<RandomMeteor>().Setup(targetPosition, _ownerStatus.InstanceId, finalDamage, areaSize);

//            delayMs = System.TimeSpan.FromSeconds(delayBetweenStrikes);
//            await UniTask.Delay(delayMs);
//        }
//    }

//    private Vector3 GetRandomScreenWorldPosition()
//    {
//        float padding = 0.1f;

//        float randomX = Random.Range(padding, 1f - padding);
//        float randomY = Random.Range(padding, 1f - padding);

//        Vector3 viewportPos = new Vector3(randomX, randomY, Mathf.Abs(mainCamera.transform.position.z));
//        Vector3 worldPos = mainCamera.ViewportToWorldPoint(viewportPos);

//        worldPos.z = 0f;

//        return worldPos;
//    }
//}