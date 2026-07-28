using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private readonly HashSet<string> _creatingKeys = new HashSet<string>();
    private readonly Dictionary<GameObject, IPoolableObject> _componentCacheDictionary = new Dictionary<GameObject, IPoolableObject>();
    private readonly Dictionary<string, List<GameObject>> _poolDictionary = new Dictionary<string, List<GameObject>>();

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
        _creatingKeys.Clear();
        _componentCacheDictionary.Clear();
        _poolDictionary.Clear();
    }

    public async UniTask<PoolResult> PoolAsync(string dataId, string key, Transform parent)
    {
        if (string.IsNullOrWhiteSpace(dataId))
        {
            Debug.LogWarning($"[오브젝트 풀링] DataId가 없어 풀링을 실패했습니다.");
            return new PoolResult(false, null);
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogWarning($"[오브젝트 풀링] Key가 없어 풀링을 실패했습니다.");
            return new PoolResult(false, null);
        }

        GameObject pooledObject = GetInactiveObject(dataId);

        if (pooledObject != null)
        {
            return new PoolResult(false, pooledObject);
        }

        while (_creatingKeys.Contains(dataId))
        {
            await UniTask.Yield();

            pooledObject = GetInactiveObject(dataId);

            if (pooledObject != null)
            {
                return new PoolResult(false, pooledObject);
            }
        }

        _creatingKeys.Add(dataId);

        GameObject instance = await ResourceManager.Instance.InstantiateGameObjectAsync(key, parent);

        _creatingKeys.Remove(dataId);

        if (instance == null)
        {
            Debug.LogError($"[오브젝트 풀링] {key} 어드레서블 에셋 인스턴스화에 실패했습니다.");
            return new PoolResult(false, null);
        }

        if (!instance.TryGetComponent(out IPoolableObject component))
        {
            Debug.LogError($"[오브젝트 풀링] 생성된 오브젝트에 IPoolableObject 컴포넌트가 없어 풀링을 중단합니다.");
            ResourceManager.Instance.TryReleaseInstance(instance);

            return new PoolResult(false, null);
        }

        if (!_poolDictionary.TryGetValue(dataId, out List<GameObject> pooledObjectList))
        {
            pooledObjectList = new List<GameObject>();
            _poolDictionary[dataId] = pooledObjectList;
        }

        pooledObjectList.Add(instance);
        _componentCacheDictionary[instance] = component;

        return new PoolResult(true, instance);
    }

    private GameObject GetInactiveObject(string dataId)
    {
        if (string.IsNullOrWhiteSpace(dataId))
        {
            Debug.LogWarning($"[오브젝트 풀링] DataId가 없어 비활성화된 오브젝트를 가져오지 못했습니다.");
            return null;
        }

        if (!_poolDictionary.TryGetValue(dataId, out List<GameObject> pooledObjectList) || pooledObjectList == null)
        {
            return null;
        }

        int pooledObjectCount = pooledObjectList.Count;

        for (int index = (pooledObjectCount - 1); index >= 0; index--)
        {
            GameObject pooledObject = pooledObjectList[index];

            if (pooledObject == null)
            {
                Debug.LogWarning($"[오브젝트 풀링] {dataId} 풀 리스트 내에 제거된 오브젝트가 발견되어 리스트에서 제거했습니다.");
                pooledObjectList.RemoveAt(index);
                continue;
            }

            if (!_componentCacheDictionary.TryGetValue(pooledObject, out IPoolableObject poolableObject))
            {
                Debug.LogWarning($"[오브젝트 풀링] {pooledObject.name} 오브젝트의 IPoolableObject 캐시 정보를 찾지 못했습니다.");
                continue;
            }

            if (poolableObject.IsActive)
            {
                continue;
            }

            return pooledObject;
        }

        return null;
    }
}