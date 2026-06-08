using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private CameraController _playerFollowCameraController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{gameObject.name}] 이미 CameraManager 인스턴스가 존재하여 생성된 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetPlayerTarget(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            Debug.LogWarning($"[{nameof(CameraManager)}] SetPlayerTarget에 전달된 playerTransform가 null입니다.");
            return;
        }

        _playerFollowCameraController.SetTrackingTarget(playerTransform);
    }
}