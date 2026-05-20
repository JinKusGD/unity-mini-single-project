using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof( Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Move")]
    [SerializeField] private float _moveSpeed = 5.0f;
   
    private const float _moveInputDeadZone = 0.25f;

    private Vector2 _moveDirection;
  
    private void OnEnable()
    {
        InputManager.Instance.BindPlayerMoveCallback(OnMoveInput, InputCallbackType.Performed);
    }

    private void OnDisable()
    {
        InputManager.Instance.UnbindPlayerMoveCallback();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();

        _moveDirection = (moveInput.sqrMagnitude > _moveInputDeadZone) ? moveInput : Vector2.zero;
    }

    private void Move()
    {
        _rigidbody.linearVelocity = _moveDirection * _moveSpeed;
    }
}



