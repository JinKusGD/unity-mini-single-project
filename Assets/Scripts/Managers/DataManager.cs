using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static class EmptyCacheDictionary<T> where T : GameData
    {
        public static readonly Dictionary<string, T> Instance = new Dictionary<string, T>();
    }

    public static DataManager Instance { get; private set; }

    private readonly Dictionary<Type, object> _dataTables = new Dictionary<Type, object>();

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

    #region 추가 로직(이곳에 추가하세요)

    public async UniTask LoadDataAsync<T>(string key) where T : GameData
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogWarning($"[{typeof(T).Name}] 어드레서블 Key가 비어 있어 데이터를 로드하지 못했습니다.");
            return;
        }

        object loadedTable = await LoadTableAsync<T>(key);

        if (loadedTable == null)
        {
            Debug.LogError($"[{typeof(T).Name}] 어드레서블 {key}에 대한 에셋을 가져오지 못해 데이터를 로드하지 못했습니다.");
            return;
        }

        _dataTables[typeof(T)] = loadedTable;
    }

    public async UniTask PreloadDataAsync()
    {
        await LoadDataAsync<AudioData>(AddressableKey.Table.Audio);
    }

    public async UniTask LoadMainDataAsync()
    {
        await LoadDataAsync<PlayerData>(AddressableKey.Table.Player);
        await LoadDataAsync<EnemyData>(AddressableKey.Table.Enemy);
        await LoadDataAsync<ExpData>(AddressableKey.Table.Exp);
        await LoadDataAsync<SkillData>(AddressableKey.Table.Skill);
        await LoadDataAsync<MeleeSkillData>(AddressableKey.Table.MeleeSkill);
        await LoadDataAsync<ProjectileSkillData>(AddressableKey.Table.ProjectileSkill);
        await LoadDataAsync<HomingSkillData>(AddressableKey.Table.HomingSkill);
        await LoadDataAsync<OrbitingSkillData>(AddressableKey.Table.OrbitingSkill);
        await LoadDataAsync<RandomTargetSkillData>(AddressableKey.Table.RandomTargetSkill);
        await LoadDataAsync<SpriteData>(AddressableKey.Table.Sprite);
        await LoadDataAsync<FieldData>(AddressableKey.Table.Field);
    }

    public bool TryGetData<T>(string dataId, out T data) where T : GameData
    {
        data = null;

        if (string.IsNullOrWhiteSpace(dataId))
        {
            Debug.LogWarning($"[{typeof(T).Name}] DataId가 비어 있어 데이터를 가져오지 못했습니다.");
            return false;
        }

        if (!_dataTables.TryGetValue(typeof(T), out object table))
        {
            Debug.LogError($"[{typeof(T).Name}] 데이터 테이블이 로드되지 않아 데이터를 가져오지 못했습니다.");
            return false;
        }

        if (table is not Dictionary<string, T> targetTable)
        {
            Debug.LogError($"[{typeof(T).Name}] 타입 캐스팅에 실패하여 데이터를 가져오지 못했습니다.");
            return false;
        }

        if (!targetTable.TryGetValue(dataId, out data))
        {
            Debug.LogError($"[{typeof(T).Name}] 테이블에 {dataId}를 가진 데이터가 없어 데이터를 가져오지 못했습니다.");
            return false;
        }

        return true;
    }

    public bool TryGetTable<T>(out Dictionary<string, T> targetTable) where T : GameData
    {
        targetTable = null;

        if (!_dataTables.TryGetValue(typeof(T), out object table))
        {
            Debug.LogError($"[{typeof(T).Name}] 데이터 테이블이 로드되지 않아 데이터를 가져오지 못했습니다.");
            return false;
        }

        if (table is not Dictionary<string, T> typedTable)
        {
            Debug.LogError($"[{typeof(T).Name}] 타입 캐스팅에 실패하여 데이터를 가져오지 못했습니다.");
            return false;
        }

        targetTable = typedTable;
        return true;
    }

    #endregion

    #region 주요 로직(건드리지 말아주세요)

    private async UniTask<Dictionary<string, T>> LoadTableAsync<T>(string key) where T : GameData
    {
        TextAsset jsonAsset = await ResourceManager.Instance.GetAssetAsync<TextAsset>(key);

        if (jsonAsset == null)
        {
            Debug.LogError($"[{key}] 어드레서블 에셋을 로드하지 못하여 테이블을 로드하지 못했습니다.");
            return EmptyCacheDictionary<T>.Instance;
        }

        try
        {
            string wrappedJsonString = $"{{\"items\":{jsonAsset.text}}}";
            JsonWrapper<T> wrapper = JsonUtility.FromJson<JsonWrapper<T>>(wrappedJsonString);

            if (wrapper == null || wrapper.items == null)
            {
                Debug.LogError($"[{key}] JSON 파싱 결과가 비어 있거나 올바르지 않아 테이블을 로드하지 못했습니다.");
                throw new InvalidOperationException();
            }

            Dictionary<string, T> dataTable = new Dictionary<string, T>();

            foreach (T item in wrapper.items)
            {
                if (string.IsNullOrWhiteSpace(item.Id))
                {
                    Debug.LogWarning($"[{key}] 아이템의 Id가 비어 있어 테이블 추가를 건너뛰었습니다.");
                    continue;
                }

                if (!dataTable.TryAdd(item.Id, item))
                {
                    Debug.LogError($"[{key}] 이미 중복된 {item.Id}가 존재하여 테이블을 로드하지 못했습니다.");
                    throw new InvalidOperationException();
                }
            }

            return dataTable;
        }
        catch
        {
            return EmptyCacheDictionary<T>.Instance;
        }
    }

    #endregion
}