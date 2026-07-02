using UnityEngine;
using UnityEngine.UI;

public class MainHUD : UIBase
{
    [SerializeField] private Slider _heightSlider;
    [SerializeField] private Text _textHeight;
    [SerializeField] private Text _textTime;

    public const float MaxHeight = 200.0f;

    private Transform _playerTransform;

    private void Awake()
    {
        _heightSlider.minValue = 0f;
        _heightSlider.maxValue = MaxHeight;
    }

    private void OnEnable()
    {
        Player player = GameManager.Instance.PlayerController;

        _playerTransform = player.transform;
    }

    private void LateUpdate()
    {
        UpdateHeightUI();
        UpdateTimeUI();
    }

    private void UpdateHeightUI()
    {
        if (_playerTransform == null)
        {
            return;
        }

        float currentHeight = _playerTransform.position.y;

        _heightSlider.value = currentHeight;
        _textHeight.text = $"{currentHeight:0}M";
    }

    private void UpdateTimeUI()
    {
        float _elapsedTime = GameManager.Instance.ElapsedTime;

        int hours = Mathf.FloorToInt(_elapsedTime / 3600f);
        int minutes = Mathf.FloorToInt((_elapsedTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(_elapsedTime % 60f);

        _textTime.text = $"{hours:00}:{minutes:00}:{seconds:00}";
    }
}