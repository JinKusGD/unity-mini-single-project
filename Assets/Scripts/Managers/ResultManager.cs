using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    public static ResultManager Instance { get; private set; }

    public Dictionary<string, float> _weaponDamageTracker = new Dictionary<string, float>();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{gameObject.name}] 이미 AudioManager 인스턴스가 존재하여 생성된 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Init()
    {
        _weaponDamageTracker.Clear();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<WeaponDamageInfo>(OnDamageRecorded);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<WeaponDamageInfo>(OnDamageRecorded);
        PrintDamageRankings();
    }

    private void OnDamageRecorded(WeaponDamageInfo data)
    {
        if (!_weaponDamageTracker.ContainsKey(data.WeaponId))
        {
            _weaponDamageTracker[data.WeaponId] = data.Damage;
            return;
        }

        _weaponDamageTracker[data.WeaponId] += data.Damage;
    }

    public string PrintDamageRankings()
    {
        if (_weaponDamageTracker.Count == 0)
        {
            Debug.Log("[ResultManager] 기록된 데미지 데이터가 없습니다.");
            return string.Empty;
        }

        List<KeyValuePair<string, float>> damageList = new List<KeyValuePair<string, float>>(_weaponDamageTracker);
        damageList.Sort(CompareDamage);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("무기별 누적 데미지 순위");

        int rank = 1;
        for (int i = 0; i < damageList.Count; i++)
        {
            KeyValuePair<string, float> entry = damageList[i];
            string weaponId = entry.Key;

            float totalDamage = entry.Value;

            string formattedDamage = totalDamage.ToString("#,##0.0");

            sb.AppendLine($"[{rank}위] {weaponId} : {formattedDamage} Damage");
            rank++;
        }

        return sb.ToString();
    }

    private int CompareDamage(KeyValuePair<string, float> frontPair, KeyValuePair<string, float> secondPair)
    {
        int compareResult = secondPair.Value.CompareTo(frontPair.Value);
        return compareResult;
    }
}
