using UnityEngine;

public class OpeningUI : UIBase
{
    [SerializeField] private UIButton _buttonStart;
    [SerializeField] private UIButton _buttonSetting;
    [SerializeField] private UIButton _buttonExit;

    private void OnEnable()
    {
        _buttonStart.BindOnClickButtonEvent(OnClickStart);
        _buttonSetting.BindOnClickButtonEvent(OnClickSetting);
        _buttonExit.BindOnClickButtonEvent(OnClickExit);
    }

    private void OnClickStart()
    {
        UIManager.Instance.OpenPopupUI(UIType.StartPopupUI);
        Debug.Log("시작선택 팝업 열림");
    }

    private void OnClickSetting()
    {
        UIManager.Instance.OpenUI(UIRootType.VeryFrontUI, UIType.SettingPopupUI);
    }

    private void OnClickExit()
    {
        Application.Quit();
    }
}
