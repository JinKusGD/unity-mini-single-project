using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private InputActionReference PlayerMoveAction;

    private Action<InputAction.CallbackContext> _bindedPlayerMoveEvent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        EnableAllAction();
    }

    private void OnDisable()
    {
        DisableAllAction();
    }

    private void OnDestroy()
    {
        UnBindAllEvent();
    }

    public void BindPlayerMoveEvent(Action<InputAction.CallbackContext> context)
    {
        if(context == null) { return; }

        _bindedPlayerMoveEvent = context;
        PlayerMoveAction.action.performed += _bindedPlayerMoveEvent;
    }

    public void UnBindPlayerMoveEvent()
    {
        if(_bindedPlayerMoveEvent == null) { return; }

        PlayerMoveAction.action.performed -= _bindedPlayerMoveEvent;
    }

    private void EnablePlayerMoveAction()
    {
        if (!PlayerMoveAction.action.enabled)
        {
            PlayerMoveAction.action.Enable();
        }
    }

    private void DisablePlayerMoveAction()
    {
        if (PlayerMoveAction.action.enabled)
        {
            PlayerMoveAction.action.Disable();
        }
    }

    private void EnableAllAction()
    {
        EnablePlayerMoveAction();
    }

    private void DisableAllAction()
    {
        DisablePlayerMoveAction();
    }

    private void UnBindAllEvent()
    {
        UnBindPlayerMoveEvent();
    }
}

