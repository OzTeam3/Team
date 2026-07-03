using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class TrapSpot : MonoBehaviour
{
    [SerializeField] private string _trapId;

    private CancellationTokenSource _cancellationTokenSource;

    private void Awake()
    {
        if (string.IsNullOrWhiteSpace(_trapId))
        {
            return;
        }

        _cancellationTokenSource = new CancellationTokenSource();
        InitTrap(_trapId).Forget();
    }
    
    public async UniTask InitTrap(string trapId)
    {
        TrapData trapData = DataManager.Instance.GetData<TrapData>(trapId);

        if (string.IsNullOrWhiteSpace(trapId))
        {
            Debug.LogError("[TrapSpot] 잘못된 trapId가 있습니다.");
            return;
        }

        if (trapData == null)
        {
            Debug.LogError($"[TrapSpot] DataManager에서 {trapId}를 찾을 수 없습니다.");
            return;
        }

        GameObject trapPrefab = await ResourceManager.Instance.InstantiateGameObjectAsync(trapData.PrefabPath,transform, cancellationToken: _cancellationTokenSource.Token);

        if (trapPrefab == null)
        {
            Debug.LogError("[TrapSpot] 프리팹을 불러오지 못했습니다.");
            return;
        }

        if (!trapPrefab.TryGetComponent(out TrapBase trapBase))
        {
            Debug.LogError($"[TrapSpot] {trapPrefab.name} 오브젝트에서 TrapBase 컴포넌트를 찾지 못했습니다.");
            return;
        }

        trapBase.Init(trapId, trapData);
        trapPrefab.SetActive(true);
    }
    private void OnDisable()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }
}