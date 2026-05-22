using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : BaseController
{
    private const float _moveInputDeadZone = 0.25f;

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
    }

    private void Move()
    {
        if (_rigidbody == null) { return; }

        if (_isDashing) { return; }

        _rigidbody.linearVelocity = _moveDirection * _moveSpeed;
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

        while (elapsedTime < _dashDuration)
        {
            Vector2 nextPosition = _rigidbody.position + (dashSpeed * Time.fixedDeltaTime * dashDirection);

            _rigidbody.MovePosition(nextPosition);

            elapsedTime += Time.fixedDeltaTime;
            await UniTask.WaitForFixedUpdate();
        }

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