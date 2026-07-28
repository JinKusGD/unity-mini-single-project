using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class DashHud : MonoBehaviour
{
    private readonly List<GameObject> _spawnedIcons = new List<GameObject>();

    private void OnEnable()
    {
        EventBus.Subscribe<DashCountInfo>(OnDashCountChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<DashCountInfo>(OnDashCountChanged);
    }

    private void OnDashCountChanged(DashCountInfo data)
    {
        DashPrefabChange(data).Forget();
    }

    private async UniTask DashPrefabChange(DashCountInfo data)
    {
        while (_spawnedIcons.Count < data.MaxCount)
        {
            GameObject newIcon = await ResourceManager.Instance.InstantiateGameObjectAsync("Prefab/Dash", transform);

            _spawnedIcons.Add(newIcon);
        }

        for (int i = 0; i < _spawnedIcons.Count; i++)
        {
            if (i < data.CurrentCount)
            {
                _spawnedIcons[i].SetActive(true);
            }
            else
            {
                _spawnedIcons[i].SetActive(false);
            }
        }
    }
}
