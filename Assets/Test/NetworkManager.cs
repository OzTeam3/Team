using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    public PlayerNetworkService PlayerNetworkService { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;

        PlayerNetworkService = new PlayerNetworkService();
    }
}
