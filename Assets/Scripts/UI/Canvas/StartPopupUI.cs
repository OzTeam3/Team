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

    private void OnDisable()
    {
        _buttonNewStart.UnBindOnClickButtonEvent(OnClickNewStart);
        _buttonContinue.UnBindOnClickButtonEvent(OnClickContinue);
        _buttonBack.UnBindOnClickButtonEvent(OnClickBack);
    }

    private async void OnClickNewStart()
    {
        GameManager.Instance.PlayNewGame();
        await MapManager.Instance.InitializeMapManager();
        await UIManager.Instance.OpenContentUIAsync(UIType.MainHUD);

        UIManager.Instance.CloseUI(UIType.StartPopupUI);
        UIManager.Instance.CloseUI(UIType.TitleUI);

        AudioController.Instance.PlayBGM(AddressableUtil.SoundPath.Bgm);
    }

    private async void OnClickContinue()
    {
        GameManager.Instance.PlayLoadGame();
        await MapManager.Instance.InitializeMapManager();
        await UIManager.Instance.OpenContentUIAsync(UIType.MainHUD);

        UIManager.Instance.CloseUI(UIType.StartPopupUI);
        UIManager.Instance.CloseUI(UIType.TitleUI);

        AudioController.Instance.PlayBGM(AddressableUtil.SoundPath.Bgm);
    }

    private void OnClickBack()
    {
        UIManager.Instance.CloseUI(UIType.StartPopupUI);
    }
}
