using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private class InputBindingInfo
    {
        public InputCallbackType CallbackType;
        public Action<InputAction.CallbackContext> Callback;
    }

    public static InputManager Instance { get; private set; }

    [SerializeField] private InputActionReference _playerMoveAction;

    private readonly InputBindingInfo _playerMoveBindingInfo = new InputBindingInfo();

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
        UnbindAllInputAllCallback();
    }

    #region 추가 로직(이곳에 추가하세요)
    public void BindPlayerMoveCallback(Action<InputAction.CallbackContext> callback, InputCallbackType callbackType)
    {
        if (callback == null) { return; }

        InputActionType inputActionType = InputActionType.PlayerMove;
        BindInputAction(callback, callbackType, inputActionType);
    }
    
    public void UnbindPlayerMoveCallback()
    {
        InputActionType inputActionType = InputActionType.PlayerMove;
        UnBindInputAction(inputActionType);
    }

    private InputBindingInfo GetInputBindingInfo(InputActionType inputActionType)
    {
        switch (inputActionType)
        {
            case InputActionType.PlayerMove:
                return _playerMoveBindingInfo;
            default:
                break;
        }

        return null;
    }

    private InputActionReference GetInputActionReference(InputActionType inputActionType)
    {
        switch (inputActionType)
        {
            case InputActionType.PlayerMove:
                return _playerMoveAction;
            default:
                break;
        }

        return null;
    }

    private void EnablePlayerMoveAction()
    {
        InputActionReference inputActionReference = GetInputActionReference(InputActionType.PlayerMove);
        
        if (inputActionReference == null) { return; }
        
        EnableAction(inputActionReference);
    }

    private void DisablePlayerMoveAction()
    {
        InputActionReference inputActionReference = GetInputActionReference(InputActionType.PlayerMove);

        if (inputActionReference == null) { return; }

        DisableAction(inputActionReference);
    }

    private void EnableAllAction()
    {
        EnablePlayerMoveAction();
    }

    private void DisableAllAction()
    {
        DisablePlayerMoveAction();
    }

    private void UnbindAllInputAllCallback()
    {
        UnbindPlayerMoveCallback();
    }
    #endregion

    #region 주요 로직(건드리지 말아주세요)
   
    private void SetupInputBindingInfo(ref InputBindingInfo inputBindingInfo, Action<InputAction.CallbackContext> callback, InputCallbackType callbackType)
    {
        if (inputBindingInfo == null) { return; }

        inputBindingInfo.CallbackType = callbackType;
        inputBindingInfo.Callback = callback;
    }

    private void BindInputCallback(InputActionReference inputActionReference, InputBindingInfo inputBindingInfo)
    {
        if (inputActionReference == null || inputActionReference.action == null || inputBindingInfo == null || inputBindingInfo.Callback == null) { return; }

        UnbindInputCallback(inputActionReference, inputBindingInfo);
     
        switch (inputBindingInfo.CallbackType)
        {
            case InputCallbackType.Started:
                inputActionReference.action.started += inputBindingInfo.Callback;
                break;
            case InputCallbackType.Performed:
                inputActionReference.action.performed += inputBindingInfo.Callback;
                break;
            case InputCallbackType.Canceled:
                inputActionReference.action.canceled += inputBindingInfo.Callback;
                break;
            default:
                break;
        }
    }

    private void UnbindInputCallback(InputActionReference inputActionReference, InputBindingInfo inputBindingInfo)
    {
        if (inputActionReference == null || inputActionReference.action == null || inputBindingInfo == null || inputBindingInfo.Callback == null) { return; }

        switch (inputBindingInfo.CallbackType)
        {
            case InputCallbackType.Started:
                inputActionReference.action.started -= inputBindingInfo.Callback;
                break;
            case InputCallbackType.Performed:
                inputActionReference.action.performed -= inputBindingInfo.Callback;
                break;
            case InputCallbackType.Canceled:
                inputActionReference.action.canceled -= inputBindingInfo.Callback;
                break;
            default:
                break;
        }
    }
    
    private void BindInputAction(Action<InputAction.CallbackContext> callback, InputCallbackType callbackType, InputActionType inputActionType)
    {
        if (callback == null || callbackType == InputCallbackType.None) { return; }

        InputBindingInfo inputBindingInfo = GetInputBindingInfo(inputActionType);
        InputActionReference inputActionReference = GetInputActionReference(inputActionType);

        if (inputBindingInfo == null || inputActionReference == null || inputActionReference.action == null) { return; }

        SetupInputBindingInfo(ref inputBindingInfo, callback, callbackType);
        BindInputCallback(inputActionReference, inputBindingInfo);
    }

    private void UnBindInputAction(InputActionType inputActionType)
    {
        InputBindingInfo inputBindingInfo = GetInputBindingInfo(inputActionType);
        InputActionReference inputActionReference = GetInputActionReference(inputActionType);

        if (inputBindingInfo == null || inputActionReference == null || inputActionReference.action == null) { return; }

        UnbindInputCallback(inputActionReference, inputBindingInfo);    
        SetupInputBindingInfo(ref inputBindingInfo, null, InputCallbackType.None);
    }

    private void EnableAction(InputActionReference inputActionReference)
    {
        if (inputActionReference == null || inputActionReference.action == null) { return; }

        if (!inputActionReference.action.enabled)
        {
            inputActionReference.action.Enable();
        }
    }

    private void DisableAction(InputActionReference inputActionReference)
    {
        if (inputActionReference == null || inputActionReference.action == null) { return; }

        if (inputActionReference.action.enabled)
        {
            inputActionReference.action.Disable();
        }
    }
   
    #endregion
}

