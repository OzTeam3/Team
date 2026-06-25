using UnityEngine;
using UnityEngine.UI;

public class SettingPopupUI : UIBase
{
    [SerializeField] private UIButton _buttonBack;
    [SerializeField] private Slider _sliderBGM;
    [SerializeField] private Slider _sliderSFX;

    private void OnEnable()
    {
        _buttonBack.BindOnClickButtonEvent(OnClickBack);

        // 버튼 바인트랑 같은 개념 이벤트를 여는 것
        _sliderBGM.onValueChanged.AddListener(OnChangeBGM);
        _sliderSFX.onValueChanged.AddListener(OnChangeSFX);

        InitSliderVolume();
    }

    private void OnClickBack()
    {
        UIManager.Instance.ClosePopupUI(UIType.SettingPopupUI);
    }

    private void OnChangeBGM(float value)
    {
        SoundManager.Instance.BGMVolume = value;
    }

    private void OnChangeSFX(float value)
    {
        SoundManager.Instance.SFXVolume = value;
    }

    private void InitSliderVolume()
    {
        _sliderBGM.value = SoundManager.Instance.BGMVolume;
        _sliderSFX.value = SoundManager.Instance.SFXVolume;
    }
}
