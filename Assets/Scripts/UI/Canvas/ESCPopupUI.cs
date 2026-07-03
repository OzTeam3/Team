using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class ESCPopupUI : UIBase
{
    [SerializeField] private UIButton _buttonBack;
    [SerializeField] private UIButton _buttonResume;
    [SerializeField] private UIButton _buttonSetting;
    [SerializeField] private UIButton _buttonTitle;

    private CancellationTokenSource _disableCancellationToken;

    private bool _isDisabled;

    private void OnEnable()
    {
        Time.timeScale = 0;
        _isDisabled = false;

        _buttonBack.BindOnClickButtonEvent(OnClickBack);
        _buttonResume.BindOnClickButtonEvent(OnClickResume);
        _buttonSetting.BindOnClickButtonEvent(OnClickSetting);
        _buttonTitle.BindOnClickButtonEvent(OnClickTitle);

        _disableCancellationToken = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        Time.timeScale = 1;

        _buttonBack.UnBindOnClickButtonEvent(OnClickBack);
        _buttonResume.UnBindOnClickButtonEvent(OnClickResume);
        _buttonSetting.UnBindOnClickButtonEvent(OnClickSetting);
        _buttonTitle.UnBindOnClickButtonEvent(OnClickTitle);

        if (_disableCancellationToken == null)
        {
            return;
        }

        _disableCancellationToken.Cancel();
        _disableCancellationToken.Dispose();
        _disableCancellationToken = null;
    }


    private void OnClickBack()
    {
        UIManager.Instance.CloseUI(UIType.ESCPopupUI);
    }

    private void OnClickResume()
    {
        UIManager.Instance.CloseUI(UIType.ESCPopupUI);
    }

    private void OnClickSetting()
    {
        UIManager.Instance.OpenVeryFrontUIAsync(UIType.SettingPopupUI, _disableCancellationToken.Token).Forget();
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
        await UIManager.Instance.OpenMainUIAsync(UIType.TitleUI, _disableCancellationToken.Token);
        
        GameManager.Instance.EndGame();
        MapManager.Instance.DisableMap();

        UIManager.Instance.CloseUI(UIType.MainHUD);
        UIManager.Instance.CloseUI(UIType.ESCPopupUI);
    }
}
