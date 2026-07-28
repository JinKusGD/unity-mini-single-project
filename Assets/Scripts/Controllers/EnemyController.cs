using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(EnemyStatus))]
public class EnemyController : BaseController
{
    [SerializeField] private EnemyStatus _enemyStatus;

    private CancellationTokenSource _cancellationTokenSource;

    private PlayerStatus _chaseTarget;
    private bool isColliding;
    private float _cooldownTimer;

    protected override void OnEnable()
    {
        base.OnEnable();
        UniTaskUtils.ClearToken(ref _cancellationTokenSource);

        _cancellationTokenSource = new CancellationTokenSource();

        SetTargetAsync().Forget();
    }

    private void Start()
    {
        _moveSpeed = _enemyStatus.MoveSpeed;
    }

    private void Update()
    {
        if (!isColliding) { return; }

        _cooldownTimer += Time.deltaTime;

        if (_cooldownTimer >= _enemyStatus.AttackCooldown)
        {
            Attack();
            _cooldownTimer -= _enemyStatus.AttackCooldown;
        }
    }

    private void FixedUpdate()
    {
        if (_chaseTarget == null || !_chaseTarget.gameObject.activeInHierarchy) { return; }

        Vector2 direction = (_chaseTarget.transform.position - transform.position).normalized;

        FlipSprite(direction);
        Move(direction);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UniTaskUtils.ClearToken(ref _cancellationTokenSource);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag(GameTags.Player)) { return; }

        Attack();

        _cooldownTimer = 0f;

        isColliding = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag(GameTags.Player)) { return; }
        
        isColliding = false;
    }

    protected override void Initialize()
    {
        if (_enemyStatus == null)
        {
            Debug.LogError($"[{gameObject.name}] EnemyStatus 컴포넌트가 누락되었습니다.");
            ObjectManager.Instance.DespawnObject(gameObject);
            return;
        }
    }

    private async UniTask SetTargetAsync()
    {
        while (_chaseTarget == null)
        {
            _chaseTarget = ObjectManager.Instance.GetPlayer();

            if (_chaseTarget != null) { break; }
            
            await UniTask.Delay(100, cancellationToken: _cancellationTokenSource.Token);
        }
    }

    private void FlipSprite(Vector2 direction)
    {
        if (direction.x == 0) { return; }

        _spriteRenderer.flipX = direction.x < 0;
    }

    private void Move(Vector2 direction)
    {
        _rigidbody.MovePosition(_rigidbody.position + _moveSpeed * Time.fixedDeltaTime * direction);
        _rigidbody.linearVelocity = Vector2.zero;
    }

    private void Attack()
    {
        if (_chaseTarget == null || _enemyStatus == null) { return; }

        _chaseTarget.TakeDamage(_enemyStatus.DataId, _enemyStatus.Power);
    }
}