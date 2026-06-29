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

    //얼리리턴
    private void SetDefaultUI()
    {
        if (_imageSelect != null)
        {
            _imageSelect.gameObject.SetActive(false);
        }
    }

    //널체크 에러 추카
    private void InitUIButton()
    {
        if (_buttonBase != null)
        {
            return;
        }
        _buttonBase = GetComponentInChildren<Button>();
    }

    //수정
    public void BindOnClickButtonEvent(UnityAction onClickCallback)
    {
        if (_buttonBase == null)
        {
            return;
        }
        _buttonBase.onClick.AddListener(onClickCallback);
    }

    public void UnBindOnClickButtonEvent(UnityAction onClickCallback)
    {
        if (_buttonBase == null)
        {
            return;
        }
        _buttonBase.onClick.RemoveListener(onClickCallback);
    }

    //널체크
    public void OnClickSetSelectUI()
    {
        if (_imageSelect != null)
        {
            bool currentActive = _imageSelect.gameObject.activeSelf;
            _imageSelect.gameObject.SetActive(!currentActive);
        }
    }

    //하드코드 빼주세요 최상위로 올려주세요.
    private void PlayHoverSound()
    {
        SoundManager.Instance.PlaySFX("Assets/Sound/Button_1");
    }

    private void PlayClickSound()
    {
        SoundManager.Instance.PlaySFX("Assets/Sound/Button_2");
    }

    public void ChangeButtonSprite(Sprite sprite)
    {
        if (_imageBase == null) return;
        _imageBase.sprite = sprite;
    }
}