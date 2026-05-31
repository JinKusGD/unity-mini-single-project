using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : BaseController
{
    private const float _moveInputDeadZone = 0.25f;

    [SerializeField] private Animator _animator;

    [Header("Move")]
    [SerializeField] private float _moveSpeed = 5.0f;

    [Header("Dash")]
    [SerializeField] private int _maxDashCount = 3;
    [SerializeField] private float _dashDistance = 4f;
    [SerializeField] private float _dashDuration = 0.15f;
    [SerializeField] private float _dashRechargeTime = 0.6f;

    private Vector2 _moveDirection;
    private bool _isDashing;
    private float _rechargeTimer;

    public int DashCount { get; private set; }

    private void OnEnable()
    {
        InputManager.Instance.BindPlayerMoveCallback(OnMoveInput, InputCallbackType.Performed);
        InputManager.Instance.BindPlayerDashCallback(OnDashInput, InputCallbackType.Started);
    }

    private void OnDisable()
    {
        InputManager.Instance.UnbindPlayerMoveCallback();
        InputManager.Instance.UnbindPlayerDashCallback();
    }

    private void Update()
    {
        RechargeDash();
    }

    private void FixedUpdate()
    {
        Move();
    }

    protected sealed override void Init()
    {
        DashCount = _maxDashCount;
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();

        _moveDirection = (moveInput.sqrMagnitude > _moveInputDeadZone) ? moveInput : Vector2.zero;
        _animator.SetFloat("Speed", _moveDirection.magnitude);
    }

    private void Move()
    {
        if (_rigidbody == null) { return; }

        if (_isDashing) { return; }

        if (_moveDirection.x != 0)
        {
            _spriteRenderer.flipX = (_moveDirection.x < 0);
        }

        _rigidbody.linearVelocity = _moveDirection * _moveSpeed;

        if (_animator.GetBool("Walk") != (_moveDirection != Vector2.zero))
        {
            _animator.SetBool("Walk", _moveDirection != Vector2.zero);
        }
    }

    private void OnDashInput(InputAction.CallbackContext context)
    {
        Dash().Forget();
    }

    private async UniTask Dash()
    {
        if (_rigidbody == null) { return; }

        if ((DashCount <= 0) || _isDashing) { return; }

        if (_moveDirection == Vector2.zero) { return; }

        DashCount--;
        _rechargeTimer = 0f;

        _isDashing = true;

        Vector2 dashDirection = _moveDirection.normalized;

        float elapsedTime = 0f;
        float dashSpeed = _dashDistance / _dashDuration;

        _spriteRenderer.enabled = false;
        _collider.enabled = false;

        while (elapsedTime < _dashDuration)
        {
            Vector2 nextPosition = _rigidbody.position + (dashSpeed * Time.fixedDeltaTime * dashDirection);

            _rigidbody.MovePosition(nextPosition);

            elapsedTime += Time.fixedDeltaTime;
            await UniTask.WaitForFixedUpdate();
        }

        _spriteRenderer.enabled = true;
        _collider.enabled = true;
        _isDashing = false;
    }

    private void RechargeDash()
    {
        if (DashCount >= _maxDashCount)
        {
            if (_rechargeTimer == 0.0f) { return; }

            _rechargeTimer = 0.0f;
            return;
        }

        _rechargeTimer += Time.deltaTime;

        while (_rechargeTimer >= _dashRechargeTime && DashCount < _maxDashCount)
        {
            DashCount = Mathf.Clamp(DashCount + 1, 0, _maxDashCount);

            _rechargeTimer -= _dashRechargeTime;
        }
    }
}