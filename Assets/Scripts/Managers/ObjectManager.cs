using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance { get; private set; }

    [SerializeField] private Transform _spawnedObjectRoot;
    
    private readonly Dictionary<int, GameObject> _spawnedObjectDictionary = new Dictionary<int, GameObject>();

    private int _instanceKeyGenerator = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public async UniTask<GameObject> SpawnMonsterAsync(string dataId, Vector3 SpawnPosition)
    {
        if (!DataManager.Instance.TryGetData<MonsterData>(dataId, out MonsterData monsterData))
        {
            return null;
        }

        PoolResult poolResult = await PoolManager.Instance.PoolAsync(dataId, monsterData.PrefabKey, _spawnedObjectRoot);

        if (!poolResult.IsSuccess)
        {
            if (poolResult.ResultObject == null)
            {
                return null;
            }
        }

        InitializeSpawnableObject(poolResult.ResultObject, SpawnPosition);

        return poolResult.ResultObject;
    }
 
    
    private void SetObjectPosition(GameObject targetObject, Vector3 spawnPosition)
    {
        targetObject.transform.position = spawnPosition;
    }

    private void InitializeSpawnableObject( GameObject targetObject, Vector3 spawnPosition)
    {
        GenerateInstanceId(targetObject);

        if (!targetObject.activeSelf)
        {
            targetObject.SetActive(true);
        }

        SetObjectPosition(targetObject, spawnPosition);
    }

    private void GenerateInstanceId(GameObject targetObject)
    {
        if (!targetObject.TryGetComponent(out ISpawnableObject component)) { return; }

        if (component.InstanceId > 0) { return; }

        int instanceId = _instanceKeyGenerator + 1;

        while (_spawnedObjectDictionary.ContainsKey(instanceId))
        {
            instanceId++;
        }

        component.SetInstanceId(instanceId);

        _spawnedObjectDictionary[instanceId] = targetObject;
        _instanceKeyGenerator = instanceId;
    }
}
