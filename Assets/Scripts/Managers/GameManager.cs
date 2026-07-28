using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float PlayTime { get; private set; }

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

    public bool IsPlay = false;

    private void Update()
    {
        if (!IsPlay) { return; }

        PlayTime += Time.deltaTime;

        float spawnDelay = Time.deltaTime * 0.001f;

        MapManager.Instance.SetSpawnDelay(spawnDelay);
    }

    public void InitManagers()
    {
        MapManager.Instance.Init();
        ObjectManager.Instance.DestroyAllObject();
        PoolManager.Instance.Init();
        ResultManager.Instance.Init();
        SkillManager.Instance.Init();
    }

    public async UniTask StartGame()
    {
        PlayTime = 0;
        IsPlay = true;
        await UIManager.Instance.OpenMainHudAsync();
        await ObjectManager.Instance.SpawnPlayerAsync("Player_001_Sylvia");
        await WarmUpExpCorePool();
        await UIManager.Instance.OpenDamageTextHudAsync();
        MapManager.Instance.ActiveMap();
        UIManager.Instance.CloseTitleUI();
        MapManager.Instance.StartSpawnLoop();
    }

    public async UniTask EndGame()
    {
        IsPlay = false;

        await UIManager.Instance.OpenTitleUIAsync();
        UIManager.Instance.CloseMainHud();
        UIManager.Instance.CloseDamageTextHud();
        InitManagers();
    }

    private async UniTask WarmUpExpCorePool()
    {
        List<GameObject> spawnedExpCoreList = new List<GameObject>(100);

        for (int i = 0; i < 100; i++)
        {
            GameObject ExpCore = await SpawnExpCore();

            spawnedExpCoreList.Add(ExpCore);
        }

        foreach (GameObject ExpCore in spawnedExpCoreList)
        {
            ObjectManager.Instance.DespawnObject(ExpCore);
        }
    }

    private async UniTask<GameObject> SpawnExpCore()
    {
        GameObject expCore = await ObjectManager.Instance.SpawnExpCoreAsync(Vector3.zero);

        if (expCore == null)
        {
            Debug.LogError($"[SpawnExpCore] SpawnExpCore 스폰 실패.");
            return null;
        }

        if (!expCore.TryGetComponent(out ExpCore exp))
        {
            Debug.LogError($"[{expCore.name}] 생성된 오브젝트에 ExpCore 컴포넌트가 없습니다.");
            ObjectManager.Instance.DestroyObject(expCore);
            return null;
        }

        return expCore;
    }
}