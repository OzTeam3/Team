using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class EndingPopupUI : UIBase
{
    [SerializeField] private UIButton _buttonTitle;
    [SerializeField] private Text _textTime;

    private void OnEnable()
    {
        Time.timeScale = 0;

        float playTime = GameManager.Instance.ElapsedTime;
        SetClearTime(playTime);

        _buttonTitle.BindOnClickButtonEvent(OnClickTitle);
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
        _buttonTitle.UnBindOnClickButtonEvent(OnClickTitle);
    }

    private void OnClickTitle()
    {
        ClickTitle().Forget();
    }

    private async UniTask ClickTitle()
    {
        UIManager.Instance.CloseUI(UIType.MainHUD);
        await UIManager.Instance.OpenMainUIAsync(UIType.TitleUI);
        GameManager.Instance.EndGame();
        MapManager.Instance.DisableMap();
        UIManager.Instance.CloseUI(UIType.EndingPopupUI);
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
