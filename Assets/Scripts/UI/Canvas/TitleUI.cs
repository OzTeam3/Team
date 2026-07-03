using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class TitleUI : UIBase
{
    [SerializeField] private UIButton _buttonStart;
    [SerializeField] private UIButton _buttonSetting;
    [SerializeField] private UIButton _buttonExit;

    private const string TitleID = "Title_001";
    private CancellationTokenSource _disableCancellationToken;

    private bool _isDisabled;

    private void OnEnable()
    {
        _isDisabled = false;

        _buttonStart.BindOnClickButtonEvent(OnClickStart);
        _buttonSetting.BindOnClickButtonEvent(OnClickSetting);
        _buttonExit.BindOnClickButtonEvent(OnClickExit);

        _disableCancellationToken = new CancellationTokenSource();

        AudioController.Instance.PlayBGM(TitleID, _disableCancellationToken.Token);
    }

    private void OnDisable()
    {
        _buttonStart.UnBindOnClickButtonEvent(OnClickStart);
        _buttonSetting.UnBindOnClickButtonEvent(OnClickSetting);
        _buttonExit.UnBindOnClickButtonEvent(OnClickExit);

        if (_disableCancellationToken == null)
        {
            return;
        }

        _disableCancellationToken.Cancel();
        _disableCancellationToken.Dispose();
        _disableCancellationToken = null;
    }

    private void OnClickStart()
    {
        UIManager.Instance.OpenPopupUIAsync(UIType.StartPopupUI, _disableCancellationToken.Token).Forget();
    }

    private void OnClickSetting()
    {
        UIManager.Instance.OpenVeryFrontUIAsync(UIType.SettingPopupUI, _disableCancellationToken.Token).Forget();
    }

    private void OnClickExit()
    {
        if (_isDisabled)
        {
            return;
        }

        _isDisabled = true;

        GameManager.Instance.ExitGame();
    }
}
