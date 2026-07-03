using System.Threading;
using UnityEngine;

public class StartPopupUI : UIBase
{
    [SerializeField] private UIButton _buttonNewStart;
    [SerializeField] private UIButton _buttonContinue;
    [SerializeField] private UIButton _buttonBack;

    private CancellationTokenSource _disableCancellationToken;

    private bool _isDisabled;

    private void OnEnable()
    {
        _isDisabled = false;

        _buttonNewStart.BindOnClickButtonEvent(OnClickNewStart);
        _buttonContinue.BindOnClickButtonEvent(OnClickContinue);
        _buttonBack.BindOnClickButtonEvent(OnClickBack);

        _disableCancellationToken = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        _buttonNewStart.UnBindOnClickButtonEvent(OnClickNewStart);
        _buttonContinue.UnBindOnClickButtonEvent(OnClickContinue);
        _buttonBack.UnBindOnClickButtonEvent(OnClickBack);

        if (_disableCancellationToken == null)
        {
            return;
        }

        _disableCancellationToken.Cancel();
        _disableCancellationToken.Dispose();
        _disableCancellationToken = null;
    }

    private async void OnClickNewStart()
    {
        if (_isDisabled)
        {
            return;
        }

        _isDisabled = true;

        GameManager.Instance.PlayNewGame();
        await MapManager.Instance.InitializeMapManager(_disableCancellationToken.Token);
        await UIManager.Instance.OpenContentUIAsync(UIType.MainHUD, _disableCancellationToken.Token);

        UIManager.Instance.CloseUI(UIType.StartPopupUI);
        UIManager.Instance.CloseUI(UIType.TitleUI);
    }

    private async void OnClickContinue()
    {
        if (_isDisabled)
        {
            return;
        }

        _isDisabled = true;

        GameManager.Instance.PlayLoadGame();
        await MapManager.Instance.InitializeMapManager(_disableCancellationToken.Token);
        await UIManager.Instance.OpenContentUIAsync(UIType.MainHUD , _disableCancellationToken.Token);
        UIManager.Instance.CloseUI(UIType.StartPopupUI);
        UIManager.Instance.CloseUI(UIType.TitleUI);
    }

    private void OnClickBack()
    {
        UIManager.Instance.CloseUI(UIType.StartPopupUI);
    }
}
