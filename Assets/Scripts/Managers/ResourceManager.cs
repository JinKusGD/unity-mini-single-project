using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    private readonly Dictionary<string, AsyncOperationHandle> _loadingHandleDictionary = new Dictionary<string, AsyncOperationHandle>();
    private readonly Dictionary<string, AsyncOperationHandle<GameObject>> _loadingInstantiateHandleDictionary = new Dictionary<string, AsyncOperationHandle<GameObject>>();

    private readonly Dictionary<string, AsyncOperationHandle> _loadedHandleDictionary = new Dictionary<string, AsyncOperationHandle>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #region 추가 로직(이곳에 추가하세요)

    public async UniTask<T> GetAssetAsync<T>(string key) where T : UnityEngine.Object
    {
        T loadedAsset = await LoadAssetAsync<T>(key);
        return loadedAsset;
    }

    public async UniTask<GameObject> InstantiateGameObjectAsync(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true)
    {
        GameObject instantiatedGameObject = await InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);

        return instantiatedGameObject;
    }

    public bool TryRelease(string key)
    {
        if (!_loadedHandleDictionary.TryGetValue(key, out AsyncOperationHandle handle))
        {
            return false;
        }

        if (!handle.IsValid())
        {
            _loadedHandleDictionary.Remove(key);

            return false;
        }

        Addressables.Release(handle);
        _loadedHandleDictionary.Remove(key);

        return true;
    }

    public bool TryReleaseInstance(GameObject instance)
    {
        if (instance == null)
        {
            return false;
        }

        bool ReleaseResult = Addressables.ReleaseInstance(instance);

        return ReleaseResult;
    }

    #endregion

    #region 주요 로직(건드리지 말아주세요)

    private async UniTask<T> LoadAssetAsync<T>(string key) where T : UnityEngine.Object
    {
        if (_loadedHandleDictionary.TryGetValue(key, out AsyncOperationHandle loadedHandle))
        {
            T result = loadedHandle.Result as T;

            return result;
        }

        if (_loadingHandleDictionary.TryGetValue(key, out AsyncOperationHandle loadingHandle))
        {
            await loadingHandle.ToUniTask();

            return loadingHandle.Result as T;
        }

        AsyncOperationHandle<T> loadHandle = Addressables.LoadAssetAsync<T>(key);
        _loadingHandleDictionary[key] = loadHandle;

        try
        {
            await loadHandle.ToUniTask();
            T loadObject = loadHandle.Result;

            if (loadObject == null)
            {
                if (loadHandle.IsValid()) Addressables.Release(loadHandle);
                return null;
            }

            _loadedHandleDictionary[key] = loadHandle;

            return loadObject;
        }
        catch
        {
            if (loadHandle.IsValid())
            {
                Addressables.Release(loadHandle);
            }

            return null;
        }
        finally
        {
            _loadingHandleDictionary.Remove(key);
        }
    }

    private async UniTask<GameObject> InstantiateAsync(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true)
    {
        if (_loadingInstantiateHandleDictionary.TryGetValue(key, out AsyncOperationHandle<GameObject> loadingHandle))
        {
            await loadingHandle.ToUniTask();

            return loadingHandle.Result;
        }

        AsyncOperationHandle<GameObject> InstantiateHandle = Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);
        _loadingInstantiateHandleDictionary[key] = InstantiateHandle;

        try
        {
            await InstantiateHandle.ToUniTask();
            GameObject instance = InstantiateHandle.Result;

            if (instance == null)
            {
                if (InstantiateHandle.IsValid())
                {
                    Addressables.ReleaseInstance(InstantiateHandle);
                }

                return null;
            }

            return instance;
        }
        catch
        {
            if (InstantiateHandle.IsValid())
            {
                Addressables.ReleaseInstance(InstantiateHandle);
            }

            return null;
        }
        finally
        {
            _loadingInstantiateHandleDictionary.Remove(key);
        }
    }

#endregion
}