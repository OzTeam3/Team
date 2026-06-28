using System;
using UnityEngine;
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

    private void SetDefaultUI()
    {
        if (_imageSelect != null)
        {
            _imageSelect.gameObject.SetActive(false);
        }
    }

    private void InitUIButton()
    {
        if (_buttonBase != null)
        {
            return;
        }
        _buttonBase = GetComponentInChildren<Button>();
    }

    public void BindOnClickButtonEvent(Action onClickCallback)
    {
        if (_buttonBase == null)
        {
            return;
        }
        _buttonBase.onClick.AddListener(new UnityEngine.Events.UnityAction(onClickCallback));
    }

    public void UnBindOnClickButtonEvent(Action onClickCallback)
    {
        if (_buttonBase == null)
        {
            return;
        }
        _buttonBase.onClick.RemoveListener(new UnityEngine.Events.UnityAction(onClickCallback));
    }

    public void OnClickSetSelectUI()
    {
        if (_imageSelect != null)
        {
            bool currentActive = _imageSelect.gameObject.activeSelf;
            _imageSelect.gameObject.SetActive(!currentActive);
        }
    }

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