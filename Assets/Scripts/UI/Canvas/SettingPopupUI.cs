using System;
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


    private List<Resolution> _availableResolutions = new List<Resolution>();

    private void OnEnable()
    {
        _buttonBack.BindOnClickButtonEvent(OnClickBack);
        _buttonBack2.BindOnClickButtonEvent(OnClickBack);

        // 버튼 바인트랑 같은 개념 이벤트를 여는 것
        _sliderBGM.onValueChanged.AddListener(OnChangeBGM);
        _sliderSFX.onValueChanged.AddListener(OnChangeSFX);
        _dropdownResolution.onValueChanged.AddListener(OnChangeResolution);

        InitSliderVolume();
        InitResolutionDropdown();
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

    private void OnChangeResolution(int index)
    {
        //드롭다운에서 몇 번째 항목을 선택했는지 받아서 그 항목에 해당하는 가로, 세로 크기를 찾아내고, 해상도를 바꿈
        Resolution selected = _availableResolutions[index];
        Screen.SetResolution(selected.width, selected.height, Screen.fullScreen);
    }

    private void InitSliderVolume()
    {
        _sliderBGM.value = SoundManager.Instance.BGMVolume;
        _sliderSFX.value = SoundManager.Instance.SFXVolume;
    }

    private void InitResolutionDropdown()
    {
        // 모니터가 지원하는 해상도 목록을 가져와서 해시셋을 이용해 중복없이 드롭다운에 채워넣는 함수.
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
