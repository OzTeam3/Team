using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class TitleUI : UIBase
{
    [SerializeField] private UIButton _buttonStart;
    [SerializeField] private UIButton _buttonSetting;
    [SerializeField] private UIButton _buttonExit;

    private CancellationTokenSource _disableCancellationToken;

    private void OnEnable()
    {
        _buttonStart.BindOnClickButtonEvent(OnClickStart);
        _buttonSetting.BindOnClickButtonEvent(OnClickSetting);
        _buttonExit.BindOnClickButtonEvent(OnClickExit);

        _disableCancellationToken = new CancellationTokenSource();

        AudioController.Instance.PlayBGM(AddressableUtil.SoundPath.Title);
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
        GameManager.Instance.ExitGame();
    }
}
