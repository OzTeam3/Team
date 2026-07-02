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
        Time.timeScale = 0;

        _buttonBack.BindOnClickButtonEvent(OnClickBack);
        _buttonResume.BindOnClickButtonEvent(OnClickResume);
        _buttonSetting.BindOnClickButtonEvent(OnClickSetting);
        _buttonTitle.BindOnClickButtonEvent(OnClickTitle);
    }

    private void OnDisable()
    {
        Time.timeScale = 1;

        _buttonBack.UnBindOnClickButtonEvent(OnClickBack);
        _buttonResume.UnBindOnClickButtonEvent(OnClickResume);
        _buttonSetting.UnBindOnClickButtonEvent(OnClickSetting);
        _buttonTitle.UnBindOnClickButtonEvent(OnClickTitle);
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
        UIManager.Instance.OpenVeryFrontUIAsync(UIType.SettingPopupUI).Forget();
    }

    private void OnClickTitle()
    {
        ClickTitle().Forget();
    }

    private async UniTask ClickTitle()
    {
        UIManager.Instance.CloseUI(UIType.MainHUD);
        UIManager.Instance.CloseUI(UIType.ESCPopupUI);
        await UIManager.Instance.OpenMainUIAsync(UIType.TitleUI);
        GameManager.Instance.EndGame();
        MapManager.Instance.DisableMap();

    }
}
