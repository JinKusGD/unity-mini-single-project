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

    private Dictionary<string, MonsterData> _monsterTable;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #region 추가 로직(이곳에 추가하세요)

    public async UniTask LoadDataAsync<T>(string key) where T : GameData
    {
        if (string.IsNullOrWhiteSpace(key)) { return; }

        if (typeof(T) == typeof(MonsterData))
        {
            _monsterTable = await LoadTableAsync<MonsterData>(key);
        }
    }

    public async UniTask LoadAllDataAsync()
    {
        await LoadDataAsync<MonsterData>(AddressableKey.Table.Monster);
    }

    public bool TryGetData<T>(string dataId, out T data) where T : GameData
    {
        if (string.IsNullOrWhiteSpace(dataId))
        {
            data = null;
            return false;
        }

        if (typeof(T) == typeof(MonsterData))
        {
            if (_monsterTable != null && _monsterTable.TryGetValue(dataId, out MonsterData monsterData))
            {
                data = monsterData as T;
                return true;
            }
        }

        data = null;
        return false;
    }

    #endregion

    #region 주요 로직(건드리지 말아주세요)

    private async UniTask<Dictionary<string, T>> LoadTableAsync<T>(string key) where T : GameData
    {
        TextAsset jsonAsset = await ResourceManager.Instance.GetAssetAsync<TextAsset>(key);

        if (jsonAsset == null)
        {
            return EmptyCacheDictionary<T>.Instance;
        }

        try
        {
            string wrappedJsonString = $"{{\"items\":{jsonAsset.text}}}";
            JsonWrapper<T> wrapper = JsonUtility.FromJson<JsonWrapper<T>>(wrappedJsonString);

            if (wrapper == null || wrapper.items == null)
            {
                return EmptyCacheDictionary<T>.Instance;
            }

            Dictionary<string, T> dataTable = new Dictionary<string, T>();

            foreach (T item in wrapper.items)
            {
                if (string.IsNullOrWhiteSpace(item.Id)) { continue; }

                if (!dataTable.TryAdd(item.Id, item))
                {
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