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

    private void OnClickNewStart()
    {
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayBGM("Assets/Sound/BGM1");

        //todo 데이터와 연동해서 저장한 기록을 불러온다. 일단 UI를 닫아 실행
        UIManager.Instance.ClosePopupUI(UIType.StartPopupUI);
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.OpeningUI);
    }

    private void OnClickContinue()
    {
        //todo 데이터와 연동해서 저장한 기록을 불러온다.
    }

    private void OnClickBack()
    {
        UIManager.Instance.ClosePopupUI(UIType.StartPopupUI);
    }
}
