using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

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
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitRootDictionary();
    }

    #region 추가 로직(이곳에 추가하세요)

    public async UniTask OpenDebugUI()
    {
        await OpenMainAsync(UIType.Debug);
    }

    public void CloseDebugUI()
    {
        CloseUI(UIType.Debug);
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

            if (targetRoot == null) { continue; }

            _rootDictionary[uiRoot] = targetRoot;
        }
    }

    private async UniTask<GameObject> CreateUIAsync(UIRoot uiRoot, UIType uiType)
    {
        _loadingSet.Add(uiType);

        try
        {
            RectTransform rootTransform = GetRootRectTransform(uiRoot);

            if (rootTransform == null) { return null; }

            string path = GetAddressableKey(uiRoot, uiType);

            GameObject createdUI = await ResourceManager.Instance.InstantiateGameObjectAsync(path, rootTransform);

            return createdUI;
        }
        finally
        {
            _loadingSet.Remove(uiType);
        }
    }

    private async UniTask OpenUIAsync(UIRoot uiRoot, UIType uiType)
    {
        if (_openedUISet.Contains(uiType) || _loadingSet.Contains(uiType)) { return; }

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
            _openedUISet.Remove(uiType);
            return;
        }

        if (_openedUISet.Remove(uiType))
        {
            uiObject.SetActive(false);
        }
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
            return null;
        }

        return rootRectTransform;
    }

    private async UniTask OpenMainAsync(UIType uIType)
    {
        await OpenUIAsync(UIRoot.Main, uIType);
    }

    private async UniTask OpenContentAsync(UIType uIType)
    {

    }

    private async UniTask OpenPopupAsync(UIType uIType)
    {

    }

    private async UniTask OpenSystemAsync(UIType uiType)
    {

    }

    private async UniTask OpenTopMostAsync(UIType uiType)
    {

    }

    #endregion
}