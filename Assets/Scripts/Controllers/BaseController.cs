using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public abstract class BaseController : MonoBehaviour, IPoolableObject
{
    [Header("Components")]
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected Rigidbody2D _rigidbody;
    [SerializeField] protected Collider2D _collider;

    [Header("Settings")]
    protected float _moveSpeed;

    public bool IsActive { get; private set; }

    public int InstanceId { get; private set; }

    protected virtual void OnEnable()
    {
        IsActive = true;
    }

    protected virtual void OnDisable()
    {
        IsActive = false;
    }

    public void Initialize(int instanceId)
    {
        InstanceId = instanceId;

        if (_spriteRenderer == null || _rigidbody == null || _collider == null)
        {
            Debug.LogError($"[{gameObject.name}] 베이스 필수 컴포넌트 중 일부가 누락되었습니다.");
            ObjectManager.Instance.DespawnObject(instanceId);
            return;
        }

        _rigidbody.gravityScale = 0f;
        _rigidbody.freezeRotation = true;

        Initialize();
    }

    protected abstract void Initialize();
}