using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
[RequireComponent(typeof(CinemachineFollow))]
[RequireComponent(typeof(CinemachineConfiner2D))]
public class CameraController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private CinemachineFollow _cinemachineFollow;
    [SerializeField] private CinemachineConfiner2D _cinemachineConfiner2D;

    [Header("Values")]
    [SerializeField] private float _orthographicSize;
    [SerializeField] private Vector3 _followOffset;

    private void Start()
    {
        InitializeSettings();
    }

    public void SetTrackingTarget(Transform targetTransform)
    {
        if(targetTransform == null)
        {
            Debug.LogWarning($"[{nameof(CameraController)}] SetTrackingTarget에 전달된 targetTransform이 null입니다.");
            return;
        }

        _cinemachineCamera.Follow = targetTransform;
    }

    private void InitializeSettings()
    {
        _cinemachineCamera.Lens.OrthographicSize = _orthographicSize;
        _cinemachineFollow.FollowOffset = _followOffset;
        _cinemachineConfiner2D.BoundingShape2D = MapManager.Instance.GetMapBoundCollider2d();
    }
}