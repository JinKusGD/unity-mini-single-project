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

    private void InitializeSpawnableObject(GameObject targetObject, Vector3 spawnPosition)
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
//public GameObject GetGameObjectByInstanceId(int instanceId)
//{
//    if (!_createdGameObjectDictionary.ContainsKey(instanceId))
//    {
//        Debug.LogWarning($"존재하지 않는 오브젝트 InstanceId [{instanceId}]");
//        return null;
//    }

//    return _createdGameObjectDictionary[instanceId];
//}

//public void RequestDestroyGameObjectByInstanceId(int instanceId)
//{
//    GameObject gameObject = GetGameObjectByInstanceId(instanceId);

//    if (gameObject == null)
//    {
//        return;
//    }

//    _createdGameObjectDictionary.Remove(instanceId);
//    Destroy(gameObject);
//}

//public void RequestDestroyAllGameObject()
//{
//    foreach (var gameObject in _createdGameObjectDictionary.Values)
//    {
//        Destroy(gameObject);
//    }

//    _createdGameObjectDictionary.Clear();
//}

//public void RequestDestroyCurrentMap()
//{
//    if (_currentMap != null)
//    {
//        Destroy(_currentMap);
//    }

//    _currentMap = null;
//}



//private bool TryGetCreatable(GameObject gameObject, out CreatableObject creatable)
//{
//    if(!gameObject.TryGetComponent(out creatable))
//    {
//        Debug.LogWarning($"생성될 수 없는 오브젝트 [{gameObject}]");
//        return false;
//    }

//    return true;
//}

//private bool TryRegisterObject(int instanceId, GameObject gameObject)
//{
//    if (_createdGameObjectDictionary.ContainsKey(instanceId))
//    {
//        Debug.LogWarning($"중복 인스턴스 Id 존재 [{instanceId}]");
//        return false;
//    }

//    _createdGameObjectDictionary.Add(instanceId, gameObject);
//    return true;
//}

//private void InitializeObject(CreatableObject creatable, int instanceId, Vector3 position)
//{
//    creatable.Init(instanceId, position);
//}

//private void AddCreatedObject(GameObject createdObject, Vector3 position)
//{
//    if (createdObject == null) return;

//    if (!TryGetCreatable(createdObject, out var creatable))
//    {
//        Debug.LogWarning($"생성될 수 없는 오브젝트 [{createdObject.name}]");
//        return;
//    }

//    int instanceId = GenerateInstanceId();

//    if (!TryRegisterObject(instanceId, createdObject)) { return; }

//    InitializeObject(creatable, instanceId, position);
//}