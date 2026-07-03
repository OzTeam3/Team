using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileUIView : MonoBehaviour
{
    [SerializeField] Text _NameText;
    [SerializeField] Text _levelText;
    [SerializeField] Slider _hpSlider;

    private PlayerViewModel _PlayerViewModel;

    public void Awake()
    {
        _hpSlider.minValue = 0;
        _hpSlider.maxValue = 100;
    }

    public void OnEnable()
    {
        PlayerViewModel playerViewModel = NetworkManager.Instance.PlayerNetworkService.GetPlayerViewModel();
        _PlayerViewModel = playerViewModel;
        _PlayerViewModel.PropertyChanged += OnPlayerModelPropertyChanged;
        _PlayerViewModel.SyncProperty();
    }

    public void OnPlayerModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(PlayerViewModel.Name):
                NameChanged(_PlayerViewModel.Name);
                break;
            case nameof(PlayerViewModel.Hp):
                HpChanged(_PlayerViewModel.Hp);
                break;
            case nameof(PlayerViewModel.Level):
                LevelChanged(_PlayerViewModel.Level);
                break;
        }
    }

    private void NameChanged(string chagedName)
    {
        _NameText.text = chagedName;
    }

    private void HpChanged(float changedHp)
    {
        _hpSlider.value = changedHp;
    }

    private void LevelChanged(int changedLevel)
    {
        _levelText.text = $"Lv.{changedLevel.ToString()}";
    }
}
