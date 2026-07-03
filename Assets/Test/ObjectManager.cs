using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance { get; private set; }

    [SerializeField] private GameObject createPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
    }

    public void CreatePlayerView(PlayerViewModel playerViewModel)
    {
        GameObject gameObject = Instantiate(createPrefab);

        if (!gameObject.TryGetComponent(out PlayerView playerView))
        {
            Debug.LogError("플레이어 뷰 아닙니다.");
            return;
        }

        playerView.SetPlayerViewModel(playerViewModel);
    }
}
