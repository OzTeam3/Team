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
        UIManager.Instance.OpenUI(UIRootType.VeryFrontUI, UIType.SettingPopupUI);
    }

    private void OnClickTitle()
    {
        SoundManager.Instance.StopBGM();
        UIManager.Instance.ClosePopupUI(UIType.ESCPopupUI);
        UIManager.Instance.ClosePopupUI(UIType.MainHUD);
        UIManager.Instance.OpenPopupUI(UIType.OpeningUI);
    }
}
