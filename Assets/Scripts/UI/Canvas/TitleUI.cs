using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleUI : UIBase
{
    [SerializeField] private UIButton _buttonStart;
    [SerializeField] private UIButton _buttonSetting;
    [SerializeField] private UIButton _buttonExit;

    private void OnEnable()
    {
        AudioController.Instance.PlayBGM(AddressableUtil.SoundPath.Title);
        _buttonStart.BindOnClickButtonEvent(OnClickStart);
        _buttonSetting.BindOnClickButtonEvent(OnClickSetting);
        _buttonExit.BindOnClickButtonEvent(OnClickExit);
    }

    private void OnClickStart()
    {
        UIManager.Instance.OpenPopupUI(UIType.StartPopupUI, this.GetCancellationTokenOnDestroy());
        Debug.Log("시작선택 팝업 열림");
    }

    private void OnClickSetting()
    {
        UIManager.Instance.OpenUI(UIRootType.VeryFrontUI, UIType.SettingPopupUI).Forget();
    }

    private void OnClickExit()
    {
        GameManager.Instance.ExitGame();
    }
}
