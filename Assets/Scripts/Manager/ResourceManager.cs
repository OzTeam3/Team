using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    private readonly Dictionary<string, AsyncOperationHandle> _loadingHandleDict = new Dictionary<string, AsyncOperationHandle>();
    private readonly Dictionary<string, AsyncOperationHandle> _loadedHandleDict = new Dictionary<string, AsyncOperationHandle>();
    private readonly Dictionary<GameObject, string> _instantiatedGameObjectDict = new Dictionary<GameObject, string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[ResourceManager:Awake] 현재 인스턴스가 존재하여 중복 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Initialize()
    {
        //최적화 작업중

        //var instancePairs = _instantiatedGameObjectDict.ToList();
        //foreach (var pair in instancePairs)
        //{
        //    if (pair.Key != null)
        //    {
        //        TryReleaseInstance(pair.Key);
        //    }
        //}
        //_instantiatedGameObjectDict.Clear();


        //var loadedPairs = _loadedHandleDict.ToList();
        //foreach (var pair in loadedPairs)
        //{
        //    if (!string.IsNullOrWhiteSpace(pair.Key))
        //    {
        //        TryRelease(pair.Key);
        //    }
        //}
        //_loadedHandleDict.Clear();


        //var loadingPairs = _loadingHandleDict.ToList();
        //foreach (var pair in loadingPairs)
        //{
        //    var handle = pair.Value;
        //    if (handle.IsValid())
        //    {
        //        if (handle.Result is GameObject instance && instance != null)
        //        {
        //            Addressables.ReleaseInstance(instance);
        //        }
        //        else
        //        {
        //            Addressables.Release(handle);
        //        }
        //    }
        //}
        //_loadingHandleDict.Clear();
    }

    public async UniTask<T> GetAssetAsync<T>(string address, CancellationToken cancellationToken = default) where T : UnityEngine.Object
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            Debug.LogError($"[ResourceManager:GetAssetAsync] address가 없어 에셋을 가져오지 못했습니다.");
            return null;
        }

        T asset = await LoadAssetAsync<T>(address, cancellationToken);

        if (asset == null)
        {
            Debug.LogError($"[ResourceManager:GetAssetAsync] 에셋을 가져오지 못했습니다.");
        }

        return asset;
    }

    public async UniTask<GameObject> InstantiateGameObjectAsync(string address, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            Debug.LogError($"[ResourceManager:InstantiateGameObjectAsync] address가 없어 오브젝트를 동적 생성하지 못했습니다.");
            return null;
        }

        GameObject instance = await InstantiateAsync<GameObject>(address, parent, instantiateInWorldSpace, trackHandle, cancellationToken);

        if (instance == null)
        {
            Debug.LogError($"[ResourceManager:InstantiateGameObjectAsync] 오브젝트를 동적 생성하지 못했습니다.");
        }

        return instance;
    }

    public bool TryRelease(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            Debug.LogError($"[ResourceManager:TryRelease] address가 없어 해제하지 못했습니다.");
            return false;
        }

        if (!_loadedHandleDict.TryGetValue(address, out AsyncOperationHandle handle))
        {
            Debug.LogWarning($"[ResourceManager:TryRelease] 로드되지 않아 해제하지 못했습니다.");
            return false;
        }

        if (!handle.IsValid())
        {
            Debug.LogWarning($"[ResourceManager:TryRelease] 유효하지 않은 핸들입니다. 해제하지 못했습니다.");
            _loadedHandleDict.Remove(address);

            return false;
        }

        Addressables.Release(handle);
        _loadedHandleDict.Remove(address);

        return true;
    }

    public bool TryReleaseInstance(GameObject instance)
    {
        if (instance == null)
        {
            Debug.LogWarning($"[ResourceManager:TryReleaseInstance] 오브젝트가 없어 해제하지 못했습니다.");
            return false;
        }

        if (!_instantiatedGameObjectDict.TryGetValue(instance, out string address))
        {
            Debug.LogWarning($"[ResourceManager:TryReleaseInstance] 어드레서블로 동적 생성된 오브젝트가 아니거나 이미 해제된 오브젝트 입니다.");
            return false;
        }

        if (!_loadedHandleDict.TryGetValue(address, out AsyncOperationHandle handle))
        {
            Debug.LogWarning($"[ResourceManager:TryReleaseInstance] 로드되지 않아 해제하지 못했습니다.");
            return false;
        }

        if (!Addressables.ReleaseInstance(instance))
        {
            Debug.LogWarning($"[ResourceManager:TryReleaseInstance] 오브젝트 해제에 실패했습니다.");
            return false;
        }

        _instantiatedGameObjectDict.Remove(instance);

        if (!handle.IsValid())
        {
            _loadedHandleDict.Remove(address);
        }

        return true;
    }

    private async UniTask<T> LoadAssetAsync<T>(string address, CancellationToken cancellationToken) where T : UnityEngine.Object
    {
        if (_loadedHandleDict.TryGetValue(address, out AsyncOperationHandle loadedHandle))
        {
            T asset = loadedHandle.Result as T;

            if (asset == null)
            {
                Debug.LogError($"[ResourceManager:LoadAssetAsync] 에셋을 로드하지 못했습니다.");
                return null;
            }

            return asset;
        }

        if (_loadingHandleDict.TryGetValue(address, out AsyncOperationHandle loadingHandle))
        {
            Debug.LogWarning($"[ResourceManager:LoadAssetAsync] 로딩 중인 작업을 대기합니다.");
            await loadingHandle.ToUniTask(cancellationToken: cancellationToken);

            T asset = loadingHandle.Result as T;

            if (asset == null)
            {
                Debug.LogError($"[ResourceManager:LoadAssetAsync] 에셋을 로드하지 못했습니다.");
                return null;
            }

            return asset;
        }

        AsyncOperationHandle<T> newHandle = Addressables.LoadAssetAsync<T>(address);
        _loadingHandleDict[address] = newHandle;

        try
        {
            await newHandle.ToUniTask();
            T asset = newHandle.Result;

            if (asset == null)
            {
                Debug.LogError($"[ResourceManager:LoadAssetAsync] 에셋을 로드하지 못했습니다.");

                if (newHandle.IsValid())
                {
                    Addressables.Release(newHandle);
                }

                return null;
            }

            _loadedHandleDict[address] = newHandle;

            return asset;
        }
        catch(Exception ex)
        {
            if (ex is not OperationCanceledException)
            {
                Debug.LogError($"[ResourceManager:LoadAssetAsync] 에셋을 로드하는 중 예외가 발생했습니다.");
            }

            if (newHandle.IsValid())
            {
                Addressables.Release(newHandle);
            }

            return null;
        }
        finally
        {
            _loadingHandleDict.Remove(address);
        }
    }

    private async UniTask<GameObject> InstantiateAsync<T>(string address, Transform parent, bool instantiateInWorldSpace, bool trackHandle, CancellationToken cancellationToken)
    {
        if (_loadingHandleDict.TryGetValue(address, out AsyncOperationHandle loadingHandle))
        {
            Debug.LogWarning($"[ResourceManager:InstantiateAsync] 로딩 중인 작업을 대기합니다.");

            await loadingHandle.ToUniTask(cancellationToken: cancellationToken);
        }

        AsyncOperationHandle<GameObject> newHandle = Addressables.InstantiateAsync(address, parent, instantiateInWorldSpace, trackHandle);
        _loadingHandleDict[address] = newHandle;

        try
        {
            await newHandle.ToUniTask();
            GameObject instance = newHandle.Result;

            if (instance == null)
            {
                Debug.LogError($"[ResourceManager:InstantiateAsync] 오브젝트를 인스턴스화 하지 못했습니다.");

                if (newHandle.IsValid())
                {
                    Addressables.ReleaseInstance(newHandle);
                }

                return null;
            }

            _loadedHandleDict[address] = newHandle;
            _instantiatedGameObjectDict[instance] = address;
            return instance;
        }
        catch (Exception ex)
        {
            if (ex is not OperationCanceledException)
            {
                Debug.LogError($"[ResourceManager:LoadAssetAsync] 에셋을 로드하는 중 예외가 발생했습니다.");
            }

            if (newHandle.IsValid())
            {
                Addressables.ReleaseInstance(newHandle);
            }

            return null;
        }
        finally
        {
            _loadingHandleDict.Remove(address);
        }
    }
}