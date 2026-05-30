using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance { get; private set; }

    [SerializeField] private Transform _spawnedObjectRoot;

    private PlayerStatus _playerObject;
    private readonly Dictionary<int, GameObject> _spawnedObjectDictionary = new Dictionary<int, GameObject>();

    private int _instanceKeyGenerator = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{gameObject.name}] ObjectManager 인스턴스가 존재하여 기존 오브젝트를 파괴했습니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public PlayerStatus GetPlayer()
    {
        if (_playerObject == null)
        {
            Debug.LogWarning($"[플레이어 참조] 플레이어가 생성되지 않아 참조를 가져오지 못했습니다.");
        }

        return _playerObject;
    }

    public async UniTask<GameObject> SpawnPlayerAsync(string dataId, Vector3 SpawnPosition)
    {
        if (string.IsNullOrWhiteSpace(dataId))
        {
            Debug.LogWarning($"[플레이어 스폰] DataId가 비어 있어 스킬 오브젝트를 스폰하지 못했습니다.");
            return null;
        }

        if (_playerObject != null)
        {
            Debug.LogWarning($"[플레이어 스폰] {_playerObject.name} 플레이어가 있어 스폰을 중단합니다.");
            return _playerObject.gameObject;
        }

        if (!DataManager.Instance.TryGetData(dataId, out PlayerData playerData))
        {
            Debug.LogError($"[플레이어 스폰] 플레이어 테이블에 {dataId}가 없어 스폰을 중단합니다.");
            return null;
        }

        GameObject playerInstance = await ResourceManager.Instance.InstantiateGameObjectAsync(playerData.PrefabKey, _spawnedObjectRoot);

        if (playerInstance == null)
        {
            Debug.LogError($"[플레이어 스폰] {playerData.PrefabKey}로 등록된 어드레서블 프리팹이 없어 스폰을 중단합니다.");
            return null;
        }

        InitializeSpawnableObject(playerInstance, SpawnPosition);

        if (!playerInstance.TryGetComponent(out PlayerStatus playerStatus))
        {
            Debug.LogError($"[플레이어 스폰] 생성된 오브젝트에 PlayerStatus 컴포넌트가 없어 스폰을 중단합니다.");

            if (ResourceManager.Instance.TryReleaseInstance(playerInstance))
            {
                return null;
            }
        }

        InitializeBaseStatus(playerData, playerStatus);

        _playerObject = playerStatus;

        return playerInstance;
    }


    public async UniTask<GameObject> SpawnMonsterAsync(string dataId, Vector3 SpawnPosition)
    {
        if (string.IsNullOrWhiteSpace(dataId))
        {
            Debug.LogWarning($"[몬스터 스폰] DataId가 비어 있어 스킬 오브젝트를 스폰하지 못했습니다.");
            return null;
        }

        if (!DataManager.Instance.TryGetData(dataId, out EnemyData monsterData))
        {
            Debug.LogError($"[몬스터 스폰] 적 테이블에서 {dataId}가 없어 스폰을 중단합니다.");
            return null;
        }

        PoolResult poolResult = await PoolManager.Instance.PoolAsync(dataId, monsterData.PrefabKey, _spawnedObjectRoot);

        if (!poolResult.IsSuccess)
        {
            if (poolResult.ResultObject == null)
            {
                Debug.LogError($"[몬스터 스폰] {dataId} 풀링에 실패하여 스폰을 중단합니다.");
                return null;
            }
        }

        InitializeSpawnableObject(poolResult.ResultObject, SpawnPosition);

        if (!poolResult.ResultObject.TryGetComponent(out EnemyStatus enemyStatus))
        {
            Debug.LogError($"[몬스터 스폰] 생성된 오브젝트에 EnemyStatus 컴포넌트가 없어 스폰을 중단합니다.");

            if (ResourceManager.Instance.TryReleaseInstance(poolResult.ResultObject))
            {
                return null;
            }
        }

        InitializeBaseStatus(monsterData, enemyStatus);

        return poolResult.ResultObject;
    }

    public async UniTask<GameObject> SpawnSkillObjectAsync(string dataId, string prefabKey, Vector3 SpawnPosition)
    {
        if (string.IsNullOrWhiteSpace(dataId))
        {
            Debug.LogWarning($"[스킬 오브젝트 스폰] DataId가 비어 있어 스킬 오브젝트를 스폰하지 못했습니다.");
            return null;
        }

        if (string.IsNullOrWhiteSpace(prefabKey))
        {
            Debug.LogWarning($"[스킬 오브젝트 스폰] {dataId}의 prefabKey가 비어 있어 스킬 오브젝트를 스폰하지 못했습니다.");
            return null;
        }

        PoolResult poolResult = await PoolManager.Instance.PoolAsync(dataId, prefabKey, _spawnedObjectRoot);

        if (!poolResult.IsSuccess)
        {
            if (poolResult.ResultObject == null)
            {
                Debug.LogError($"[스킬 오브젝트 스폰] {dataId} 풀링에 실패하여 스폰을 중단합니다.");
                return null;
            }
        }

        InitializeSpawnableObject(poolResult.ResultObject, SpawnPosition);

        return poolResult.ResultObject;
    }

    public void DespawnObject(int instanceId)
    {
        GameObject targetObject = FindObject(instanceId);

        if (targetObject == null)
        {
            Debug.LogWarning($"[오브젝트 디스폰] 인스턴스 ID {instanceId}에 해당하는 오브젝트가 없어 디스폰하지 못했습니다.");
            return;
        }

        if (!targetObject.activeSelf)
        {
            Debug.LogWarning($"[오브젝트 디스폰] 인스턴스 ID {instanceId}는 이미 디스폰된 오브젝트 입니다.");
            return;
        }

        targetObject.SetActive(false);
    }

    public void DespawnObject(GameObject targetObject)
    {
        if (targetObject == null)
        {
            Debug.LogWarning($"[오브젝트 디스폰] {targetObject} 오브젝트가 없어 디스폰하지 못했습니다.");
            return;
        }

        if (!targetObject.activeSelf)
        {
            Debug.LogWarning($"[오브젝트 디스폰] {targetObject}는 이미 디스폰된 오브젝트 입니다.");
            return;
        }

        targetObject.SetActive(false);
    }

    public void DestroyObject(int instanceId)
    {
        GameObject targetObject = FindObject(instanceId);

        if (targetObject == null)
        {
            Debug.LogWarning($"[오브젝트 파괴] 인스턴스 ID {instanceId}에 해당하는 오브젝트가 없어 파괴하지 못했습니다.");
            return;
        }

        _spawnedObjectDictionary.Remove(instanceId);
        Destroy(targetObject);
    }

    public void DestroyObject(GameObject targetObject)
    {
        if (targetObject == null)
        {
            Debug.LogWarning($"[오브젝트 파괴] {targetObject} 오브젝트가 없어 파괴하지 못했습니다.");
            return;
        }

        targetObject.SetActive(false);
    }

    private void SetObjectPosition(GameObject targetObject, Vector3 spawnPosition)
    {
        if (targetObject == null)
        {
            Debug.LogError($"[오브젝트 배치] 오브젝트가 없어 위치를 설정하지 못했습니다.");
            return;
        }

        targetObject.transform.position = spawnPosition;
    }

    private void InitializeSpawnableObject(GameObject targetObject, Vector3 spawnPosition)
    {
        if (targetObject == null)
        {
            Debug.LogError($"[오브젝트 설정] 스폰된 오브젝트가 없어 기본 설정을 하지 못했습니다.");
            return;
        }

        GenerateInstanceId(targetObject);

        if (!targetObject.activeSelf)
        {
            targetObject.SetActive(true);
        }

        SetObjectPosition(targetObject, spawnPosition);
    }

    private void InitializeBaseStatus(UnitData unitData, BaseStatus baseStatus)
    {
        if (unitData == null)
        {
            Debug.LogError($"[기본 스탯 설정] 설정할 데이터가 없어 스탯을 설정하지 못했습니다.");
            return;
        }

        if (baseStatus == null)
        {
            Debug.LogError($"[기본 스탯 설정] BaseStatus 컴포넌트가 없어 스탯을 설정하지 못했습니다.");
            return;
        }

        baseStatus.InitStatus(unitData.MaxHp, unitData.AttackPower, unitData.MoveSpeed);
    }

    private void GenerateInstanceId(GameObject targetObject)
    {
        if (targetObject == null)
        {
            Debug.LogError($"[인스턴스 ID 설정] 오브젝트가 없어 인스턴스 ID를 설정하지 못했습니다.");
            return;
        }

        if (!targetObject.TryGetComponent(out ISpawnableObject component))
        {
            Debug.LogWarning($"[{targetObject.name}] 오브젝트에 ISpawnableObject 인터페이스가 없어 인스턴스 ID를 생성하지 못했습니다.");
            return;
        }

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

    private GameObject FindObject(int instanceId)
    {
        if (!_spawnedObjectDictionary.TryGetValue(instanceId, out GameObject targetObject))
        {
            Debug.LogWarning($"[오브젝트 찾기] 인스턴스 ID {instanceId}에 해당하는 오브젝트를 찾지 못했습니다.");
            return null;
        }

        return targetObject;
    }
}