using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button _buttonBase;
    [SerializeField] private Text _textBase;
    [SerializeField] private Image _imageBase;
    [SerializeField] private Image _imageSelect;
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private string _clickSoundPath = AddressableUtil.SoundPath.Button1;
    [SerializeField] private string _hoverSoundPath = AddressableUtil.SoundPath.Button2;

    private Vector3 _originalScale;

    private void Awake()
    {
        InitUIButton();
        SetDefaultUI();
        _originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        BindOnClickButtonEvent(OnClickSetSelectUI);
        BindOnClickButtonEvent(PlayClickSound);
    }

    private void OnDisable()
    {
        _buttonBase.onClick.RemoveAllListeners();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = _originalScale * _hoverScale;
        PlayHoverSound();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = _originalScale;
    }

    private void SetDefaultUI()
    {
        if (_imageSelect == null)
        {
            return;
        }
        _imageSelect.gameObject.SetActive(false);

    }

    private void InitUIButton()
    {
        if (_buttonBase != null)
        {
            Debug.Log("[UIButton:InitButton] 버튼 컴포넌트가 할당되어 있습니다.");
            return;
        }
        _buttonBase = GetComponentInChildren<Button>();
    }

    public void BindOnClickButtonEvent(UnityAction onClickCallback)
    {
        _buttonBase.onClick.AddListener(onClickCallback);
    }

    public void UnBindOnClickButtonEvent(UnityAction onClickCallback)
    {
        _buttonBase.onClick.RemoveListener(onClickCallback);
    }

    public void OnClickSetSelectUI()
    {
        if (_imageSelect == null)
        {
            return;
        }

        bool currentActive = _imageSelect.gameObject.activeSelf;
        _imageSelect.gameObject.SetActive(!currentActive);
    }

    private void PlayHoverSound()
    {
        AudioController.Instance.PlaySFX(_hoverSoundPath);
    }

    private void PlayClickSound()
    {
        AudioController.Instance.PlaySFX(_clickSoundPath);
    }

    public void ChangeButtonSprite(Sprite sprite)
    {
        if (_imageBase == null)
        {
            return;
        }
        _imageBase.sprite = sprite;
    }
}