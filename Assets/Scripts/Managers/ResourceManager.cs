using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    private readonly Dictionary<string, AsyncOperationHandle> _loadingHandleDictionary = new Dictionary<string, AsyncOperationHandle>();
    private readonly Dictionary<string, AsyncOperationHandle> _loadedHandleDictionary = new Dictionary<string, AsyncOperationHandle>();

    private readonly Dictionary<string, AsyncOperationHandle<GameObject>> _loadingInstantiateHandleDictionary = new Dictionary<string, AsyncOperationHandle<GameObject>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{gameObject.name}] ResourceManager 인스턴스가 존재하여 기존 오브젝트를 파괴했습니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public async UniTask<T> GetAssetAsync<T>(string key) where T : Object
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogWarning($"[에셋 로드] Key가 없어 에셋을 가져오지 못했습니다.");
            return null;
        }

        T loadedAsset = await LoadAssetAsync<T>(key);

        if (loadedAsset == null)
        {
            Debug.LogError($"[에셋 로드] 어드레서블 {key} 경로의 에셋 로드에 실패하였습니다.");
        }

        return loadedAsset;
    }

    public async UniTask<GameObject> InstantiateGameObjectAsync(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogWarning($"[오브젝트 동적 생성] Key가 없어 오브젝트 동적 생성에 실패했습니다.");
            return null;
        }

        GameObject instantiatedGameObject = await InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);
        
        if (instantiatedGameObject == null)
        {
            Debug.LogError($"[오브젝트 동적 생성] 어드레서블 {key} 오브젝트 동적 생성에 실패했습니다.");
        }

        return instantiatedGameObject;
    }

    public bool TryRelease(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogWarning($"[에셋 해제] Key가 없어 에셋 해제에 실패했습니다.");
            return false;
        }

        if (!_loadedHandleDictionary.TryGetValue(key, out AsyncOperationHandle handle))
        {
            Debug.LogWarning($"[에셋 해제] 로드된 {key} 에셋이 없어 해제하지 못했습니다.");
            return false;
        }

        if (!handle.IsValid())
        {
            Debug.LogError($"[에셋 해제] {key}의 어드레서블 핸들이 유효하지 않아 캐시를 제거합니다.");
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
            Debug.LogWarning($"[오브젝트 파괴] 오브젝트가 없어 파괴에 실패했습니다.");
            return false;
        }

        if (!Addressables.ReleaseInstance(instance))
        {
            Debug.LogError($"[오브젝트 파괴] {instance.name} 오브젝트 파괴에 실패했습니다.");
            return false;
        }

        return true;
    }

    private async UniTask<T> LoadAssetAsync<T>(string key) where T : UnityEngine.Object
    {
        if (_loadedHandleDictionary.TryGetValue(key, out AsyncOperationHandle loadedHandle))
        {
            T result = loadedHandle.Result as T;

            if (result == null)
            {
                Debug.LogError($"[비동기 에셋 로드] 캐시된 핸들의 에셋 타입 캐스팅에 실패하여 {key} 에셋을 반환하지 못했습니다.");
                return null;
            }

            return result;
        }

        if (_loadingHandleDictionary.TryGetValue(key, out AsyncOperationHandle loadingHandle))
        {
            Debug.LogWarning($"[비동기 에셋 로드] {key} 에셋이 이미 로딩 중이므로 대기합니다.");
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
                Debug.LogError($"[비동기 에셋 로드] {key} 비동기 에셋 로드에 실패했습니다.");

                if (loadHandle.IsValid()) Addressables.Release(loadHandle);

                return null;
            }

            _loadedHandleDictionary[key] = loadHandle;

            return loadObject;
        }
        catch
        {
            Debug.LogError($"[비동기 에셋 로드] {key} 비동기 에셋 로드 중 시스템 예외가 발생하였습니다.");

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
            Debug.LogWarning($"[비동기 인스턴스화] {key} 프리팹이 이미 인스턴스화 진행 중이므로 대기합니다.");
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
                Debug.LogError($"[비동기 인스턴스화] {key} 비동기 인스턴스화에 실패했습니다");

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
            Debug.LogError($"[비동기 인스턴스화] {key} 비동기 인스턴스화 중 시스템 예외가 발생하였습니다.");

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
}