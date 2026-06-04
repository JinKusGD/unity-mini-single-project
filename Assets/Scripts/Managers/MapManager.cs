using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [SerializeField] private float _spawnInterval = 0.5f;
    [SerializeField] private float _padding = 0.1f;

    private CancellationTokenSource _cancellationTokenSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{gameObject.name}] MapManager 인스턴스가 존재하여 기존 오브젝트를 파괴했습니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
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

            ObjectManager.Instance.SpawnMonsterAsync("Monster_001_Slime", spawnPosition).Forget();

            await UniTaskUtils.DelayAsync(_spawnInterval, token);
        }
    }
}
