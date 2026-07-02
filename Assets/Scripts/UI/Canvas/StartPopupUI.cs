using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class StartPopupUI : UIBase
{
    [SerializeField] private UIButton _buttonNewStart;
    [SerializeField] private UIButton _buttonContinue;
    [SerializeField] private UIButton _buttonBack;

    private void OnEnable()
    {
        _buttonNewStart.BindOnClickButtonEvent(OnClickNewStart);
        _buttonContinue.BindOnClickButtonEvent(OnClickContinue);
        _buttonBack.BindOnClickButtonEvent(OnClickBack);
    }

    private async void OnClickNewStart()
    {
        AudioController.Instance.PlayBGM(AddressableUtil.SoundPath.Bgm1);
        GameManager.Instance.ResetGame();
        UIManager.Instance.ClosePopupUI(UIType.StartPopupUI);
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.TitleUI);
        GameManager.Instance.InitPlayer();

        await UIManager.Instance.OpenContentUIAsync(UIType.MainHUD);
        MainHUD.Instance.StartHUD();
    }

    private async void OnClickContinue()
    {
        GameManager.Instance.LoadGame();
        GameManager.Instance.InitPlayer();
        UIManager.Instance.ClosePopupUI(UIType.StartPopupUI);
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.TitleUI);

        await UIManager.Instance.OpenContentUIAsync(UIType.MainHUD);
        MainHUD.Instance.StartHUD();
    }

    private void OnClickBack()
    {
        UIManager.Instance.ClosePopupUI(UIType.StartPopupUI);
    }
}
