using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Button _buttonBase;
    [SerializeField] private Text _textBase;
    [SerializeField] private Image _imageBase;

     private const string _clickSoundId = "Button1_001";
     private const string _hoverSoundId = "Button2_001";

    private void Awake()
    {
        InitUIButton();
    }

    private void OnEnable()
    {
        BindOnClickButtonEvent(PlayClickSound);
    }

    private void OnDisable()
    {
        UnBindOnClickButtonEvent(PlayClickSound);
    }

    private void InitUIButton()
    {
        if (_buttonBase != null)
        {
            return;
        }

        _buttonBase = GetComponentInChildren<Button>();

        if (_buttonBase == null)
        {
            Debug.LogError("[UIButton:InitUIButton] 버튼 컴포넌트를 가져오지 못했습니다.");
        }
    }

    public void BindOnClickButtonEvent(UnityAction onClickCallback)
    {
        _buttonBase.onClick.AddListener(onClickCallback);
    }

    public void UnBindOnClickButtonEvent(UnityAction onClickCallback)
    {
        _buttonBase.onClick.RemoveListener(onClickCallback);
    }

    private void PlayClickSound()
    {
        AudioController.Instance.PlaySFX(_clickSoundId);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioController.Instance.PlaySFX(_hoverSoundId);
    }
}