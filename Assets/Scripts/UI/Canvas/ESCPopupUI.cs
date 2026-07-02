using Cysharp.Threading.Tasks;
using UnityEngine;

public class ESCPopupUI : UIBase
{
    [SerializeField] private UIButton _buttonBack;
    [SerializeField] private UIButton _buttonResume;
    [SerializeField] private UIButton _buttonSetting;
    [SerializeField] private UIButton _buttonTitle;

    private void OnEnable()
    {
        _buttonBack.BindOnClickButtonEvent(OnClickBack);
        _buttonResume.BindOnClickButtonEvent(OnClickResume);
        _buttonSetting.BindOnClickButtonEvent(OnClickSetting);
        _buttonTitle.BindOnClickButtonEvent(OnClickTitle);
    }

    private void OnClickBack()
    {

        UIManager.Instance.ClosePopupUI(UIType.ESCPopupUI);
    }

    private void OnClickResume()
    {
        UIManager.Instance.ClosePopupUI(UIType.ESCPopupUI);
    }

    private void OnClickSetting()
    {
        UIManager.Instance.OpenUI(UIRootType.VeryFrontUI, UIType.SettingPopupUI).Forget();
    }

    private void OnClickTitle()
    {
        AudioController.Instance.StopBGM();
        UIManager.Instance.ClosePopupUI(UIType.ESCPopupUI);
        UIManager.Instance.CloseContentUI(UIType.MainHUD);
        UIManager.Instance.OpenPopupUI(UIType.TitleUI, this.GetCancellationTokenOnDestroy());
        GameManager.Instance.ResetGame();
    }
}
