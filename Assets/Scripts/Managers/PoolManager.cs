using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private readonly Dictionary<GameObject, IPoolableObject> _componentCacheDictionary = new Dictionary<GameObject, IPoolableObject>();
    private readonly Dictionary<string, List<GameObject>> _poolDictionary = new Dictionary<string, List<GameObject>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);

            return;
        }

        Instance = this;
    }

    public async UniTask<PoolResult> PoolAsync(string dataId, string key, Transform parent)
    {
        GameObject pooledObject = GetInactiveObject(dataId);

        if (pooledObject != null)
        {
            return new PoolResult(false, pooledObject);
        }

        GameObject instance = await ResourceManager.Instance.InstantiateGameObjectAsync(key, parent);

        if (instance == null)
        {
            return new PoolResult(false, null);
        }

        if (!instance.TryGetComponent(out IPoolableObject component))
        {
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
                pooledObjectList.RemoveAt(index);

                continue;
            }

            if (!_componentCacheDictionary.TryGetValue(pooledObject, out IPoolableObject poolableObject) || poolableObject.IsActive)
            {
                continue;
            }

            return pooledObject;
        }

        return null;
    }
}