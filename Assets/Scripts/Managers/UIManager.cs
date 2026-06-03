using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private RectTransform _hudRoot;
    [SerializeField] private RectTransform _mainRoot;
    [SerializeField] private RectTransform _contentRoot;
    [SerializeField] private RectTransform _popupRoot;
    [SerializeField] private RectTransform _systemRoot;
    [SerializeField] private RectTransform _topMostRoot;

    private readonly HashSet<UIType> _loadingSet = new HashSet<UIType>();
    private readonly HashSet<UIType> _openedUISet = new HashSet<UIType>();

    private readonly Dictionary<UIRoot, RectTransform> _rootDictionary = new Dictionary<UIRoot, RectTransform>();
    private readonly Dictionary<UIType, GameObject> _cachedUIDictionary = new Dictionary<UIType, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{gameObject.name}] UIManager 인스턴스가 존재하여 기존 오브젝트를 파괴했습니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitRootDictionary();
    }

    private void Start()
    {
        OpenSplashUI();
    }

    #region 추가 로직(이곳에 추가하세요)

    public void ShowDamagePopupText(float damage, Vector3 targetPosition, Color textColor)
    {
        if(!_cachedUIDictionary.TryGetValue(UIType.DamagePopup, out GameObject uiObjecct))
        {
            Debug.LogError($"[DamageTextHud] UI가 캐싱 되지 않았습니다.");
            return;
        }

        if(!uiObjecct.TryGetComponent(out UIBase uIBase))
        {
            Debug.LogError($"[SpawnDamageTextHud] UIBase 컴포넌트가 없습니다.");
            return;
        }

        if (uIBase is not HudDamagePopup hudDamagePopup)
        {
            Debug.LogError($"[SpawnDamageTextHud] UIBase 컴포넌트가 DamageTextHud 컴포넌트가 아닙니다.");
            return;
        }

        hudDamagePopup.ShowDamagePopupText(damage, targetPosition, textColor).Forget();
    }

    public void OpenSplashUI()
    {
        OpenTopMostAsync(UIType.Splash).Forget();
    }

    public void CloseSplashUI()
    {
        CloseUI(UIType.Splash);
    }

    public async UniTask OpenTitleUI()
    {
        await OpenMainAsync(UIType.Title);
    }

    public void CloseTitleUI()
    {
        CloseUI(UIType.Title);
    }

    public void OpenDamageTextHud()
    {
        OpenHudAsync(UIType.DamagePopup).Forget();
    }

    public void CloseDamageTextHud()
    {
        CloseUI(UIType.DamagePopup);
    }

    #endregion

    #region 주요 로직(건드리지 말아주세요)

    private void InitRootDictionary()
    {
        Array rootArray = Enum.GetValues(typeof(UIRoot));

        foreach (UIRoot uiRoot in rootArray)
        {
            RectTransform targetRoot = null;

            switch (uiRoot)
            {
                case UIRoot.Hud:
                    targetRoot = _hudRoot;
                    break;
                case UIRoot.Main:
                    targetRoot = _mainRoot;
                    break;
                case UIRoot.Content:
                    targetRoot = _contentRoot;
                    break;
                case UIRoot.Popup:
                    targetRoot = _popupRoot;
                    break;
                case UIRoot.System:
                    targetRoot = _systemRoot;
                    break;
                case UIRoot.TopMost:
                    targetRoot = _topMostRoot;
                    break;
            }

            if (targetRoot == null) 
            {
                Debug.LogWarning($"[UI 루트 캐싱] 인스펙터에 {uiRoot}에 해당하는 RectTransform이 할당되지 않았습니다.");
                continue;
            }

            _rootDictionary[uiRoot] = targetRoot;
        }
    }

    private async UniTask<GameObject> CreateUIAsync(UIRoot uiRoot, UIType uiType)
    {
        _loadingSet.Add(uiType);

        try
        {
            RectTransform rootTransform = GetRootRectTransform(uiRoot);

            if (rootTransform == null)
            {
                Debug.LogError($"[UI 비동기 생성] {uiRoot} 루트를 찾지 못해 UI 비동기 생성에 실패했습니다.");
                return null;
            }

            string path = GetAddressableKey(uiRoot, uiType);

            GameObject createdUI = await ResourceManager.Instance.InstantiateGameObjectAsync(path, rootTransform);
          
            if (createdUI == null)
            {
                Debug.LogError($"[UI 비동기 생성] {path} UI 인스턴스화에 실패하여 UI 비동기 생성에 실패했습니다.");
            }

            return createdUI;
        }
        catch
        {
            Debug.LogError($"[UI 비동기 생성] {uiType} 생성 중 시스템 예외가 발생했습니다.");
            return null;
        }
        finally
        {
            _loadingSet.Remove(uiType);
        }
    }

    private async UniTask OpenUIAsync(UIRoot uiRoot, UIType uiType)
    {
        if (_openedUISet.Contains(uiType) || _loadingSet.Contains(uiType))
        {
            Debug.LogWarning($"[UI 열기] {uiType}가 이미 열려있거나 로딩 중이므로 열지 못했습니다.");
            return; 
        }

        _openedUISet.Add(uiType);

        GameObject cachedUI = GetCachedUI(uiType);

        if(cachedUI != null)
        {
            cachedUI.SetActive(true);
            return;
        }
        
        GameObject createdUI = await CreateUIAsync(uiRoot, uiType);

        if (createdUI == null) 
        {
            Debug.LogError($"[UI 열기] {uiType} UI 비동기 생성에 실패하여 여는 작업을 취소합니다.");
            _openedUISet.Remove(uiType);
            return;
        }

        createdUI.SetActive(true);
        _cachedUIDictionary[uiType] = createdUI;
    }

    private void CloseUI(UIType uiType)
    {
        if (!_cachedUIDictionary.TryGetValue(uiType, out GameObject uiObject) || uiObject == null)
        {
            Debug.LogWarning($"[UI 닫기] {uiType}가 존재하지 않아 닫지 못했습니다.");
            _openedUISet.Remove(uiType);
            return;
        }

        if (!_openedUISet.Remove(uiType))
        {
            Debug.LogWarning($"[UI 닫기] {uiType}는 열려있지 않아 닫지 못했습니다.");
            return;
        }

        uiObject.SetActive(false);
    }

    private string GetAddressableKey(UIRoot uiRoot, UIType uiType)
    {
        string path = $"UI/{uiRoot}/{uiType}";

        return path;
    }

    private GameObject GetCachedUI(UIType uiType)
    {
        if (!_cachedUIDictionary.TryGetValue(uiType, out GameObject uiObject))
        {
            return null;
        }

        return uiObject;
    }

    private RectTransform GetRootRectTransform(UIRoot uiRoot)
    {
        if (!_rootDictionary.TryGetValue(uiRoot, out RectTransform rootRectTransform))
        {
            Debug.LogWarning($"[UI 루트 캐싱 반환] 캐싱되지 않은 {uiRoot}입니다.");
            return null;
        }

        return rootRectTransform;
    }

    private async UniTask OpenHudAsync(UIType uIType)
    {
        await OpenUIAsync(UIRoot.Hud, uIType);
    }

    private async UniTask OpenMainAsync(UIType uIType)
    {
        await OpenUIAsync(UIRoot.Main, uIType);
    }

    private async UniTask OpenContentAsync(UIType uIType)
    {
        await OpenUIAsync(UIRoot.Content, uIType);
    }

    private async UniTask OpenPopupAsync(UIType uIType)
    {
        await OpenUIAsync(UIRoot.Popup, uIType);
    }

    private async UniTask OpenSystemAsync(UIType uiType)
    {
        await OpenUIAsync(UIRoot.System, uiType);
    }

    private async UniTask OpenTopMostAsync(UIType uiType)
    {
        await OpenUIAsync(UIRoot.TopMost, uiType);
    }

    #endregion
}