using System.Diagnostics;

public class PlayerData
{
    public string Name;
    public float Hp;
    public int Level;

    public PlayerData(string name, float hp, int level)
    {
        Name = name;
        Hp = hp;
        Level = level;
    }
}

public class PlayerNetworkService
{
    private PlayerViewModel _playerViewModel;

    public PlayerViewModel GetPlayerViewModel(PlayerData playerData = null)
    {
        if (_playerViewModel == null)
        {
            _playerViewModel = CreatePlayerViewModel(playerData);
        }

        return _playerViewModel;
    }

    public void RequestPlayerLevelUp()
    {
        if (_playerViewModel == null)
        {
            return;
        }

        int currentLevel = _playerViewModel.Level;
        int targetLevel = currentLevel + 1;

        _playerViewModel.Level = targetLevel;
    }

    private PlayerViewModel CreatePlayerViewModel(PlayerData playerData)
    {
        if (playerData == null)
        {
            playerData = new PlayerData("DefaultName", 0, 0);
        }

        PlayerViewModel playerViewModel = new PlayerViewModel();
        playerViewModel.Name = playerData.Name;
        playerViewModel.Hp = playerData.Hp;
        playerViewModel.Level = playerData.Level;

        return playerViewModel;
    }
}
