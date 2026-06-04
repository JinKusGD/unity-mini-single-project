using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{gameObject.name}] GameManager 인스턴스가 존재하여 기존 오브젝트를 파괴했습니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public async UniTask StartGame()
    {
        await ObjectManager.Instance.SpawnPlayerAsync("Player_001_Sylvia", Vector3.zero);
        await UIManager.Instance.OpenDamageTextHudAsync();
        UIManager.Instance.CloseTitleUI();
        MapManager.Instance.StartSpawnLoop();
    }
}