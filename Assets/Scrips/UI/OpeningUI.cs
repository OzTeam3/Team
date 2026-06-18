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
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.OpeningUI);
    }

    private void OnClickSetting()
    {

    }

    private void OnClickExit()
    {

    }
}
