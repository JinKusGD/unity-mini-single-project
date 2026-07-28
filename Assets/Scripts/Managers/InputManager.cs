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

    [Header("Input Actions")]
    [SerializeField] private InputActionReference _playerMoveAction;
    [SerializeField] private InputActionReference _playerDashAction;

    private readonly InputBindingInfo _playerMoveBindingInfo = new InputBindingInfo();
    private readonly InputBindingInfo _playerDashBindingInfo = new InputBindingInfo();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{gameObject.name}] 이미 AudioManager 인스턴스가 존재하여 생성된 오브젝트를 파괴합니다.");
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
        if (callback == null)
        {
            Debug.LogWarning($"[{InputActionType.PlayerMove}] 콜백 함수가 비어있어 바인딩을 진행하지 못했습니다.");
            return;
        }

        InputActionType inputActionType = InputActionType.PlayerMove;
        BindInputAction(callback, callbackType, inputActionType);
    }

    public void UnbindPlayerMoveCallback()
    {
        InputActionType inputActionType = InputActionType.PlayerMove;
        UnBindInputAction(inputActionType);
    }

    public void BindPlayerDashCallback(Action<InputAction.CallbackContext> callback, InputCallbackType callbackType)
    {
        if (callback == null)
        {
            Debug.LogWarning($"[{InputActionType.PlayerDash}] 콜백 함수가 비어있어 바인딩을 진행하지 못했습니다.");
            return;
        }

        InputActionType inputActionType = InputActionType.PlayerDash;
        BindInputAction(callback, callbackType, inputActionType);
    }

    public void UnbindPlayerDashCallback()
    {
        InputActionType inputActionType = InputActionType.PlayerDash;
        UnBindInputAction(inputActionType);
    }


    private InputBindingInfo GetInputBindingInfo(InputActionType inputActionType)
    {
        switch (inputActionType)
        {
            case InputActionType.PlayerMove:
                return _playerMoveBindingInfo;
            case InputActionType.PlayerDash:
                return _playerDashBindingInfo;
            default:
                Debug.LogError($"[{inputActionType}] 정의되지 않은 InputActionType 으로 InputBindingInfo를 찾지 못했습니다.");
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
            case InputActionType.PlayerDash:
                return _playerDashAction;
            default:
                Debug.LogError($"[{inputActionType}] 정의되지 않은 InputActionType 으로 InputActionReference를 찾지 못했습니다.");
                break;
        }

        return null;
    }

    private void EnablePlayerMoveAction()
    {
        InputActionReference inputActionReference = GetInputActionReference(InputActionType.PlayerMove);

        if (inputActionReference == null)
        {
            Debug.LogError($"[{InputActionType.PlayerMove}] InputActionReference가 없어 액션을 활성화하지 못했습니다.");
            return;
        }

        EnableAction(inputActionReference);
    }

    private void DisablePlayerMoveAction()
    {
        InputActionReference inputActionReference = GetInputActionReference(InputActionType.PlayerMove);

        if (inputActionReference == null)
        {
            Debug.LogError($"[{InputActionType.PlayerMove}] InputActionReference가 없어 액션을 비활성화하지 못했습니다.");
            return;
        }

        DisableAction(inputActionReference);
    }

    private void EnablePlayerDashAction()
    {
        InputActionReference inputActionReference = GetInputActionReference(InputActionType.PlayerDash);

        if (inputActionReference == null)
        {
            Debug.LogError($"[{InputActionType.PlayerDash}] InputActionReference가 없어 액션을 활성화하지 못했습니다.");
            return;
        }

        EnableAction(inputActionReference);
    }

    private void DisablePlayerDashAction()
    {
        InputActionReference inputActionReference = GetInputActionReference(InputActionType.PlayerDash);

        if (inputActionReference == null)
        {
            Debug.LogError($"[{InputActionType.PlayerDash}] InputActionReference가 없어 액션을 비활성화하지 못했습니다.");
            return;
        }

        DisableAction(inputActionReference);
    }

    private void EnableAllAction()
    {
        EnablePlayerMoveAction();
        EnablePlayerDashAction();
    }

    private void DisableAllAction()
    {
        DisablePlayerMoveAction();
        DisablePlayerDashAction();
    }

    private void UnbindAllInputAllCallback()
    {
        UnbindPlayerMoveCallback();
        UnbindPlayerDashCallback();
    }

    #endregion

    #region 주요 로직(건드리지 말아주세요)

    private void SetupInputBindingInfo(ref InputBindingInfo inputBindingInfo, Action<InputAction.CallbackContext> callback, InputCallbackType callbackType)
    {
        if (inputBindingInfo == null)
        {
            Debug.LogError($"[바인딩 정보 세팅] nputBindingInfo가 없어 세팅하지 못했습니다.");
            return;
        }

        inputBindingInfo.CallbackType = callbackType;
        inputBindingInfo.Callback = callback;
    }

    private void BindInputCallback(InputActionReference inputActionReference, InputBindingInfo inputBindingInfo)
    {
        if (inputActionReference == null || inputActionReference.action == null || inputBindingInfo == null || inputBindingInfo.Callback == null)
        {
            Debug.LogWarning($"[인풋 콜백 등록] 참조 요소를 확인하지 못해 콜백 등록에 실패했습니다.");
            return;
        }

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
                Debug.LogError($"[{inputActionReference.action.name}] 정의되지 않은 InputCallbackType으로 콜백 등록에 실패했습니다.");
                break;
        }
    }

    private void UnbindInputCallback(InputActionReference inputActionReference, InputBindingInfo inputBindingInfo)
    {
        if (inputActionReference == null || inputActionReference.action == null || inputBindingInfo == null || inputBindingInfo.Callback == null)
        {
            Debug.LogWarning($"[인풋 콜백 해제] 참조 요소를 확인하지 못해 콜백 해제에 실패했습니다.");
            return;
        }

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
                Debug.LogError($"[{inputActionReference.action.name}] 정의되지 않은 InputCallbackType으로 콜백 해제에 실패했습니다.");
                break;
        }
    }

    private void BindInputAction(Action<InputAction.CallbackContext> callback, InputCallbackType callbackType, InputActionType inputActionType)
    {
        if (callback == null || callbackType == InputCallbackType.None)
        {
            Debug.LogWarning($"[{inputActionType}] 올바르지 않은 콜백 또는 None 타입으로 액션 등록을 실패했습니다.");
            return;
        }

        InputBindingInfo inputBindingInfo = GetInputBindingInfo(inputActionType);
        InputActionReference inputActionReference = GetInputActionReference(inputActionType);

        if (inputBindingInfo == null || inputActionReference == null || inputActionReference.action == null)
        {
            Debug.LogError($"[{inputActionType}] InputBindingInfo 또는 InputActionReference가 유효하지 않아 액션 등록에 실패했습니다.");
            return;
        }

        SetupInputBindingInfo(ref inputBindingInfo, callback, callbackType);
        BindInputCallback(inputActionReference, inputBindingInfo);
    }

    private void UnBindInputAction(InputActionType inputActionType)
    {
        InputBindingInfo inputBindingInfo = GetInputBindingInfo(inputActionType);
        InputActionReference inputActionReference = GetInputActionReference(inputActionType);

        if (inputBindingInfo == null || inputActionReference == null || inputActionReference.action == null)
        {
            Debug.LogError($"[{inputActionType}] InputBindingInfo 또는 InputActionReference가 유효하지 않아 액션 해제에 실패했습니다.");
            return;
        }

        UnbindInputCallback(inputActionReference, inputBindingInfo);
        SetupInputBindingInfo(ref inputBindingInfo, null, InputCallbackType.None);
    }

    private void EnableAction(InputActionReference inputActionReference)
    {
        if (inputActionReference == null || inputActionReference.action == null)
        {
            Debug.LogError($"[액션 활성화] 인풋 액션 또는 내부 이벤트가 비어있어 액션을 활성화하지 못했습니다.");
            return;
        }

        if (!inputActionReference.action.enabled)
        {
            inputActionReference.action.Enable();
        }
    }

    private void DisableAction(InputActionReference inputActionReference)
    {
        if (inputActionReference == null || inputActionReference.action == null)
        {
            Debug.LogError($"[액션 비활성화] 인풋 액션 또는 내부 이벤트가 비어있어 액션을 비활성화하지 못했습니다.");
            return;
        }

        if (inputActionReference.action.enabled)
        {
            inputActionReference.action.Disable();
        }
    }

    #endregion
}