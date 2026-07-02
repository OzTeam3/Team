using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopupUI : UIBase
{
    [SerializeField] private UIButton _buttonBack;
    [SerializeField] private UIButton _buttonBack2;
    [SerializeField] private Slider _sliderBGM;
    [SerializeField] private Slider _sliderSFX;
    [SerializeField] private Dropdown _dropdownResolution;

    private readonly List<Resolution> _availableResolutions = new List<Resolution>();

    private void Awake()
    {
        InitResolutionDropdown();
    }

    private void OnEnable()
    {
        _buttonBack.BindOnClickButtonEvent(OnClickBack);
        _buttonBack2.BindOnClickButtonEvent(OnClickBack);

        _sliderBGM.onValueChanged.AddListener(OnChangeBGM);
        _sliderSFX.onValueChanged.AddListener(OnChangeSFX);

        _dropdownResolution.onValueChanged.AddListener(OnChangeResolution);

        InitSliderVolume();
    }

    private void OnDisable()
    {
        _buttonBack.UnBindOnClickButtonEvent(OnClickBack);
        _buttonBack2.UnBindOnClickButtonEvent(OnClickBack);

        _sliderBGM.onValueChanged.RemoveListener(OnChangeBGM);
        _sliderSFX.onValueChanged.RemoveListener(OnChangeSFX);

        _dropdownResolution.onValueChanged.RemoveListener(OnChangeResolution);
    }

    private void OnClickBack()
    {
        UIManager.Instance.CloseUI(UIType.SettingPopupUI);
    }

    private void OnChangeBGM(float value)
    {
        AudioController.Instance.BGMVolume = value;
    }

    private void OnChangeSFX(float value)
    {
        AudioController.Instance.SFXVolume = value;
    }

    private void OnChangeResolution(int index)
    {
        Resolution selected = _availableResolutions[index];
        Screen.SetResolution(selected.width, selected.height, Screen.fullScreen);
    }

    private void InitSliderVolume()
    {
        _sliderBGM.value = AudioController.Instance.BGMVolume;
        _sliderSFX.value = AudioController.Instance.SFXVolume;
    }

    private void InitResolutionDropdown()
    {
        _availableResolutions.Clear();

        List<string> options = new List<string>();
        HashSet<string> addedLabels = new HashSet<string>();
        int currentIndex = 0;

        Resolution[] allResolutions = Screen.resolutions;
        for (int i = 0; i < allResolutions.Length; i++)
        {
            string label = $"{allResolutions[i].width} x {allResolutions[i].height}";

            if (addedLabels.Contains(label))
            {
                continue;
            }

            addedLabels.Add(label);
            options.Add(label);
            _availableResolutions.Add(allResolutions[i]);

            if (allResolutions[i].width == Screen.currentResolution.width
                && allResolutions[i].height == Screen.currentResolution.height)
            {
                currentIndex = _availableResolutions.Count - 1;
            }
        }

        _dropdownResolution.ClearOptions();
        _dropdownResolution.AddOptions(options);
        _dropdownResolution.value = currentIndex;
        _dropdownResolution.RefreshShownValue();
    }
}
