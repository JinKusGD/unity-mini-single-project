using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [SerializeField] private Grid _map;
    [SerializeField] private float _spawnInterval = 0.7f;
    [SerializeField] private float _padding = 0.1f;

    [SerializeField] private Collider2D mapBoundColider;
    public MapSize MapSize { get; private set; }

    private string _currentMap;
    private CancellationTokenSource _cancellationTokenSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{gameObject.name}] 이미 AudioManager 인스턴스가 존재하여 생성된 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        MapSize = new MapSize(-189.5f, 487.5f, -33.2f, 557.2f);
    }


    public Collider2D GetMapBoundCollider2d()
    {
        return mapBoundColider;
    }

    private List<string> spawnMonsterList = new List<string>();

    float spawnPower = 1;

    private void Update()
    {
        if (!GameManager.Instance.IsPlay) { return; }

        spawnPower +=  0.003f * Time.deltaTime;
    }
    public void Init()
    {
        _currentMap = null;
        spawnPower = 1;
        DeactivateMap();
        StopSpawnLoop();
    }

    public void ActiveMap()
    {
        _map.gameObject.SetActive(true);
    }

    public void SetSpawnDelay(float delay)
    {
        _spawnInterval = Mathf.Max(0.1f, _spawnInterval - delay);
    }

    public void DeactivateMap()
    {
        _map.gameObject.SetActive(false);
    }

    public void ChangedField(string fieldMap)
    {
        if(_currentMap == fieldMap) { return; }

        if(!DataManager.Instance.TryGetData(fieldMap, out FieldData data))
        { 

        }

        spawnMonsterList.Clear();
        spawnMonsterList.Add(data.SpwanMonsterID);

        AudioManager.Instance.PlayBGM(data.AudioClipId);

        _currentMap = fieldMap;

        UIManager.Instance.OpenFieldPopupAsync(data.Name).Forget();
    }

    public void StartSpawnLoop()
    {
        StopSpawnLoop();
        _cancellationTokenSource = new CancellationTokenSource();

        SpawnLoopAsync(_cancellationTokenSource.Token).Forget();
    }

    public void StopSpawnLoop()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }

    public Vector3 GetRandomPositionOutsideCamera(float padding = 0.1f)
    {
        if (Camera.main == null)
        {
            return Vector3.zero;
        }

        int edge = Random.Range(0, 4);

        float viewportX = 0f;
        float viewportY = 0f;

        switch (edge)
        {
            case 0:
                viewportX = Random.Range(0f - padding, 1f + padding);
                viewportY = 1f + padding;
                break;
            case 1:
                viewportX = Random.Range(0f - padding, 1f + padding);
                viewportY = 0f - padding;
                break;
            case 2:
                viewportX = 0f - padding;
                viewportY = Random.Range(0f - padding, 1f + padding);
                break;
            case 3:
                viewportX = 1f + padding;
                viewportY = Random.Range(0f - padding, 1f + padding);
                break;
        }

        Vector3 worldPosition = Camera.main.ViewportToWorldPoint(new Vector3(viewportX, viewportY, 0f));

        worldPosition.z = 0f;

        return worldPosition;
    }

    private async UniTask SpawnLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Vector3 spawnPosition = GetRandomPositionOutsideCamera(_padding);

            string monsterId = GetRandomMonster();
            ObjectManager.Instance.SpawnMonsterAsync(monsterId, spawnPosition, spawnPower).Forget();

            await UniTaskUtils.DelayAsync(_spawnInterval, token);
        }
    }

    private string GetRandomMonster()
    {
        if (spawnMonsterList.Count == 0)
        {
            return null;
        }

        return spawnMonsterList[Random.Range(0, spawnMonsterList.Count)];
    }
}
