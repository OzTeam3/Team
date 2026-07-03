using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class EndingPopupUI : UIBase
{
    [SerializeField] private UIButton _buttonTitle;
    [SerializeField] private Text _textTime;
    
    private CancellationTokenSource _disableCancellationToken;
    
    private bool _isDisabled;

    private void OnEnable()
    {
        Time.timeScale = 0;
        _isDisabled = false;

        float playTime = GameManager.Instance.ElapsedTime;
        SetClearTime(playTime);

        _buttonTitle.BindOnClickButtonEvent(OnClickTitle);

        _disableCancellationToken = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        Time.timeScale = 1;

        _buttonTitle.UnBindOnClickButtonEvent(OnClickTitle);
        
        if (_disableCancellationToken == null)
        {
            return;
        }

        _disableCancellationToken.Cancel();
        _disableCancellationToken.Dispose();
        _disableCancellationToken = null;
    }

    private void OnClickTitle()
    {
        if (_isDisabled)
        {
            return;
        }

        ClickTitle().Forget();
        _isDisabled = true;
    }

    private async UniTask ClickTitle()
    {
        UIManager.Instance.CloseUI(UIType.MainHUD);
        await UIManager.Instance.OpenMainUIAsync(UIType.TitleUI, _disableCancellationToken.Token);
        MapManager.Instance.DisableMap();
        UIManager.Instance.CloseUI(UIType.EndingPopupUI);
    }

    private void SetClearTime(float clearTime)
    {
        int hours = Mathf.FloorToInt(clearTime / 3600f);
        int minutes = Mathf.FloorToInt((clearTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(clearTime % 60f);

        if (_textTime != null)
        {
            _textTime.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
    }
}
