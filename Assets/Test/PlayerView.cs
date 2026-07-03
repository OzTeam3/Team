using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] MeshRenderer _meshRenderer;

    private readonly List<Color> _colors = new List<Color>();

    private PlayerViewModel _PlayerViewModel;

    private void Awake()
    {
        _colors.Add(Color.white);
        _colors.Add(Color.black);
        _colors.Add(Color.red);
        _colors.Add(Color.blue);
    }

    public void SetPlayerViewModel(PlayerViewModel playerViewModel)
    {
        _PlayerViewModel = playerViewModel;
        _PlayerViewModel.PropertyChanged += OnPlayerModelPropertyChanged;
    }

    public void OnPlayerModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(PlayerViewModel.Level):
                LevelUpLogic();
                break;
        }
    }

    private void LevelUpLogic()
    {
        int index = Random.Range(0, _colors.Count);

        _meshRenderer.material.color = _colors[index];
    }
}
