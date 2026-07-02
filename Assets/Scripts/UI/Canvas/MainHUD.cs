using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MainHUD : UIBase
{
    [SerializeField] private Slider _heightSlider;
    [SerializeField] private Text _textHeight;
    [SerializeField] private Text _textTime;

    public static MainHUD Instance { get; private set; }
    public float _elapsedTime { get; private set; }

    private float _maxHeight = 150f;
    private float _startY;
    private bool _isRunning = false;
    private bool _isStartHeightSet = false;
    private GroundDetector _groundDetector;
    private Transform _playerTransform;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        CheckAndBindPlayer();
        InitHUD();
    }

    private void Update()
    {
        OnClickEscape();

        if (!_isRunning || _playerTransform == null || !_isStartHeightSet)
        {
            return;
        }

        _elapsedTime += Time.deltaTime;
        UpdateHeightUI();
        UpdateTimeUI();
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerCreated -= BindPlayer;
            GameManager.Instance.PlayerCreated += BindPlayer;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerCreated -= BindPlayer;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void BindPlayer(Transform playerTransform)
    {
        UnbindGroundDetector();

        _playerTransform = playerTransform;
        _isStartHeightSet = false;

        _groundDetector = playerTransform.GetComponentInChildren<GroundDetector>();
        if (_groundDetector != null)
        {
            _groundDetector.GroundTriggeredEvent += OnPlayerGroundTriggered;
        }
    }

    private void CheckAndBindPlayer()
    {
        if (GameManager.Instance != null && GameManager.Instance.GetPlayerTransform() != null)
        {
            BindPlayer(GameManager.Instance.GetPlayerTransform());
        }
    }

    public void StartHUD()
    {
        _playerTransform = null;
        _isStartHeightSet = false;
        CheckAndBindPlayer();
        _elapsedTime = 0f;
        _isRunning = true;

        InitHUD();
    }

    public void StopHUD()
    {
        _isRunning = false;
    }

    private void InitHUD()
    {
        _maxHeight = 150f;

        _heightSlider.minValue = 0f;
        _heightSlider.maxValue = _maxHeight;

        _heightSlider.value = 0f;
        _textHeight.text = "0M";
    }

    private void UpdateHeightUI()
    {
        if (_playerTransform == null)
        {
            return;
        }

        float currentHeight = _playerTransform.position.y - _startY;

        _heightSlider.value = currentHeight;
        _textHeight.text = string.Format("{0:0}M", currentHeight);
    }

    private void UpdateTimeUI()
    {
        int hours = Mathf.FloorToInt(_elapsedTime / 3600f);
        int minutes = Mathf.FloorToInt((_elapsedTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(_elapsedTime % 60f);

        _textTime.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    private void OnClickEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenEscapePopup();
        }
    }

    private void OpenEscapePopup()
    {
        UIManager.Instance.OpenPopupUI(UIType.ESCPopupUI, this.GetCancellationTokenOnDestroy());
    }

    private void OnPlayerGroundTriggered(bool isGrounded)
    {
        if (isGrounded && !_isStartHeightSet)
        {
            _startY = _playerTransform.position.y;
            _isStartHeightSet = true;

            Debug.Log($"[MainHUD] 최초 착지 확인! 기준 StartY 설정 완료: {_startY}M");

            UnbindGroundDetector();
        }
    }

    private void UnbindGroundDetector()
    {
        if (_groundDetector != null)
        {
            _groundDetector.GroundTriggeredEvent -= OnPlayerGroundTriggered;
            _groundDetector = null;
        }
    }
}