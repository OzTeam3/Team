using System;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    [SerializeField] private Button _buttonBase;
    [SerializeField] private Text _textBase;
    [SerializeField] private Image _imageBase;
    [SerializeField] private Image _imageSelect;

    private void Awake()
    {
        InitUIButton();
        SetDefaultUI();
    }

    private void OnEnable()
    {
        BindOnClickButtonEvent(OnClickSetSelectUI);
    }

    private void OnDisable()
    {
        _buttonBase.onClick.RemoveAllListeners();
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
        if( _buttonBase != null )
        {
            return;
        }

        _buttonBase = GetComponentInChildren<Button>();
    }

    public void BindOnClickButtonEvent(Action onClickCallback)
    {
        if(_buttonBase == null)
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
        if( _imageSelect != null)
        {
            bool currentActive = _imageSelect.gameObject.activeSelf;
            _imageSelect.gameObject.SetActive(!currentActive);
        }
    }

    public void ChangeButtonSprite(Sprite sprite)
    {
        if (_imageBase == null) return;
        _imageBase.sprite = sprite;
    }
}
