using UnityEngine;
using Cysharp.Threading.Tasks;


public class TrapSpot : MonoBehaviour
{
    [SerializeField] private string _trapId;

    private void Start()
    {
        if (!string.IsNullOrEmpty(_trapId))
        {
            DelayedSpawn().Forget();
        }
    }

    private async UniTask DelayedSpawn()
    {
        await UniTask.Delay(1000);

        InitTrap(_trapId).Forget();
    }

    public async UniTask InitTrap(string trapId)
    {
        TrapData trapData = DataManager.Instance.GetData<TrapData>(trapId);
        if (trapData == null)
        {
            Debug.LogWarning($"[TrapSpot] DataManager에서 {trapId}를 찾을 수 없습니다.");
            return;
        }

        GameObject trapPrefab = await ResourceManager.Instance.InstantiateGameObjectAsync(
            trapData.PrefabPath,
            transform,
            false,
            true
        );

        if (trapPrefab == null)
        {
            Debug.LogError($"[TrapSpot] {trapData.PrefabPath} 에셋을 불러오지 못했습니다.");
            return;
        }

        if (trapPrefab.TryGetComponent<SpinTrap>(out SpinTrap spinTrap)) spinTrap.Init(trapId);
        else if (trapPrefab.TryGetComponent<JumpPad>(out JumpPad jumpPad)) jumpPad.Init(trapId);
        else if (trapPrefab.TryGetComponent<PendulumTrap>(out PendulumTrap pendulumTrap)) pendulumTrap.Init(trapId);
        else if (trapPrefab.TryGetComponent<RollingLogTrap>(out RollingLogTrap rollingTrap)) rollingTrap.Init(trapId);
        else if (trapPrefab.TryGetComponent<RotatingPlatformTrap>(out RotatingPlatformTrap rotatingTrap)) rotatingTrap.Init(trapId);
        else if (trapPrefab.TryGetComponent<FanTrap>(out FanTrap fanTrap)) fanTrap.Init(trapId);
        else if (trapPrefab.TryGetComponent<BlinkFloor>(out BlinkFloor blinkFloor)) blinkFloor.Init(trapId);
        else if (trapPrefab.TryGetComponent<FallingPlatform>(out FallingPlatform fallingPlatform)) fallingPlatform.Init(trapId);
        else
        {
            Debug.LogWarning($"[TrapEntity] {trapPrefab.name} 오브젝트에서 초기화할 트랩 스크립트를 찾지 못했습니다.");
        }
    }
}