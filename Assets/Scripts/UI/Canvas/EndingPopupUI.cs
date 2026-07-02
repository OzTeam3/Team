using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class EndingPopupUI : UIBase
{
    [SerializeField] private UIButton _buttonTitle;
    [SerializeField] private Text _textTime;

    private void OnEnable()
    {
        _buttonTitle.BindOnClickButtonEvent(OnClickTitle);

        if (MainHUD.Instance != null)
        {
            SetClearTime(MainHUD.Instance._elapsedTime);
        }
    }

    private void OnClickTitle()
    {
        UIManager.Instance.CloseContentUI(UIType.MainHUD);
        UIManager.Instance.ClosePopupUI(UIType.EndingPopupUI);
        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.TitleUI).Forget();
    }

    private void SetClearTime(float clearTime)
    {
        int hours = Mathf.FloorToInt(clearTime / 3600f);
        int minutes = Mathf.FloorToInt((clearTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(clearTime % 60f);

        if (_textTime != null)
        {
            _textTime.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
    }

}
