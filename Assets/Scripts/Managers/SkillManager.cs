using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    private readonly List<string> skillPoolId = new List<string>();
    private readonly Dictionary<string, ILevelable> _cachedSkillObjects = new Dictionary<string, ILevelable>();
    private readonly List<string> _resultRandomSkillList = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{gameObject.name}] SkillManager 인스턴스가 존재하여 기존 오브젝트를 파괴했습니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        skillPoolId.Clear();
        _cachedSkillObjects.Clear();

        skillPoolId.Add("Skill_001_Pyroblast_Lv01");
        skillPoolId.Add("Skill_002_OrbitalFlame_Lv01");
        skillPoolId.Add("Skill_003_HeatSeeker_Lv01");
        skillPoolId.Add("Skill_005_FireWhip_Lv01");
        skillPoolId.Add("Skill_006_Eruption_Lv01");
    }
    
    public bool HasNextSkill()
    {
        bool hasNextSkill = skillPoolId.Count > 0;

        return hasNextSkill;
    }

    public void AddPoolNextLevelId(string currentSkillId, string NextSkillId)
    {
        skillPoolId.Remove(currentSkillId);

        if(string.IsNullOrWhiteSpace(NextSkillId)) { return; }

        skillPoolId.Add(NextSkillId);
    }

    public List<string> GetRandomSkillList(int count)
    {
        _resultRandomSkillList.Clear();

        if (!HasNextSkill())
        {
            for (int i = 0; i < count; i++)
            {
                _resultRandomSkillList.Add(null);
            }

            return _resultRandomSkillList;
        }

        List<string> copySkillPoolList = new List<string>(skillPoolId);

        for (int i = 0; i < count; i++)
        {
            if (copySkillPoolList.Count > 0)
            {
                int randomIndex = Random.Range(0, copySkillPoolList.Count);

                _resultRandomSkillList.Add(copySkillPoolList[randomIndex]);
                copySkillPoolList.RemoveAt(randomIndex);
            }
            else
            {
                _resultRandomSkillList.Add(null);
            }
        }

        return _resultRandomSkillList;
    }

    public async UniTask<ILevelable> GetSkillObject(string address)
    {
        if (_cachedSkillObjects.TryGetValue(address, out ILevelable levelable))
        {
            return levelable;
        }

        GameObject spawnedSkillObject = await ObjectManager.Instance.SpawnSkillAsync(address);

        if (spawnedSkillObject.TryGetComponent(out levelable))
        {
            _cachedSkillObjects[address] = levelable;
        }

        return  null;
    }
}
