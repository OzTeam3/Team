using UnityEngine;

public class GameManagerTest : MonoBehaviour
{
    public static GameManagerTest Instance { get; private set; }

    [SerializeField] private GameObject _playerProfileUI;
    [SerializeField] private GameObject _TestUI;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestCreatePlayer();
        }
    }

    public void RequestCreatePlayer()
    {
        PlayerData playerData = new PlayerData("OzMagic", 100, 1);
        PlayerViewModel playerViewModel = NetworkManager.Instance.PlayerNetworkService.GetPlayerViewModel(playerData);
        ObjectManager.Instance.CreatePlayerView(playerViewModel);
        _playerProfileUI.SetActive(true);
        _TestUI.SetActive(true);
    }
}
