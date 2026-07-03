using UnityEngine;
using UnityEngine.UI;

public class LevelUpButton : MonoBehaviour
{
    [SerializeField] Button _levelUp;

    private void OnEnable()
    {
        if (_levelUp == null)
        { 
            return;
        }

        _levelUp.onClick.RemoveAllListeners();
        _levelUp.onClick.AddListener(LevelUp);
    }

    private void OnDisable()
    {
        if (_levelUp == null)
        {
            return;
        }

        _levelUp.onClick.RemoveAllListeners();
    }

    private void LevelUp()
    {
        NetworkManager.Instance.PlayerNetworkService.RequestPlayerLevelUp();
    }
}
