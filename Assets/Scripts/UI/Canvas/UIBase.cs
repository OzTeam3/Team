using UnityEngine;
using UnityEngine.Events;

public class UIBase : MonoBehaviour
{
   protected void Bind(ref UIButton a, UnityAction b)
    {
        if(a == null)
        {
            Debug.Log("에러");
        }

        a.BindOnClickButtonEvent(b);
    }
}

public class UIPopup : UIBase
{
    [SerializeField] private UIButton _buttonNewStart;

    private void Awake()
    {
        Bind(ref _buttonNewStart, OnClickNewStart);
    }

    private void OnClickNewStart()
    {
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayBGM("Assets/Sound/BGM1");

        //todo 데이터와 연동해서 저장한 기록을 불러온다. 일단 UI를 닫아 실행
        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.MainHUD);
        UIManager.Instance.ClosePopupUI(UIType.StartPopupUI);
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.OpeningUI);
    }
}
