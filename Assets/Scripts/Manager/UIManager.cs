using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private RectTransform Canvas_BgRoot;
    [SerializeField] private RectTransform Canvas_MainRoot;
    [SerializeField] private RectTransform Canvas_ContentRoot;
    [SerializeField] private RectTransform Canvas_PopupRoot;
    [SerializeField] private RectTransform Canvas_VeryFrontRoot;

    private readonly Dictionary<UIType, UIBase> _createUIDictionary = new Dictionary<UIType, UIBase>();
    private readonly HashSet<UIType> _openedUIDictionary = new HashSet<UIType>();

    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[UIManager:Awake] 현재 인스턴스가 존재하여 중복 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public async UniTask<UIBase> OpenUI(UIRootType uiRootType, UIType uiType, bool isInitialHide = false, CancellationToken cancellationToken = default)
    {
        return await OpenUIAsync(uiRootType, uiType, isInitialHide, cancellationToken);
    }

    public void CloseUI(UIRootType uiRootType, UIType uiType, CancellationToken cancellationToken = default)
    {
        if (!_openedUIDictionary.Contains(uiType))
        {
            return;
        }

        UIBase openUi = _createUIDictionary[uiType];

        if (openUi == null)
        {
            _createUIDictionary.Remove(uiType);
            return;
        }
        openUi.gameObject.SetActive(false);
        _openedUIDictionary.Remove(uiType);
    }

    // Forget 전용 - 반환 타입이 void라서 .Forget()을 붙이고 싶어도 붙일 데가 없음
    public void OpenContentUI(UIType uiType, CancellationToken cancellationToken = default)
    {
        OpenUI(UIRootType.ContentUI, uiType, cancellationToken: cancellationToken).Forget();
    }

    // Await 전용 - UniTask<UIBase>를 반환하니 호출부가 반드시 await로 받게 유도됨
    public async UniTask<UIBase> OpenContentUIAsync(UIType uiType, CancellationToken cancellationToken = default)
    {
        return await OpenUIAsync(UIRootType.ContentUI, uiType, false, cancellationToken);
    }

    public void OpenPopupUI(UIType uiType, CancellationToken cancellationToken = default)
    {
        OpenUI(UIRootType.PopupUI, uiType, cancellationToken: cancellationToken).Forget();
    }

    public async UniTask<UIBase> OpenPopupUIAsync(UIType uiType, CancellationToken cancellationToken = default)
    {
        return await OpenUIAsync(UIRootType.PopupUI, uiType, false, cancellationToken);
    }

    public void CloseContentUI(UIType uiType, CancellationToken cancellationToken = default)
    {
        CloseUI(UIRootType.ContentUI, uiType);
    }

    public void ClosePopupUI(UIType uiType, CancellationToken cancellationToken = default)
    {
        CloseUI(UIRootType.PopupUI, uiType);
    }

    public async UniTask OpenFadeUI(CancellationToken cancellationToken, Action onComplete = null)
    {
        var uiBase = await OpenUI(UIRootType.VeryFrontUI, UIType.FadePopupUI, cancellationToken: cancellationToken);

        if (uiBase is FadePopupUI fadePopupUI)
        {
            await fadePopupUI.Fade(onComplete);
        }
    }

    #region 메인 로직
    private async UniTask<UIBase> CreateUI(UIRootType uiRootType, UIType uiType, CancellationToken cancellationToken = default)
    {
        string path = AddressableUtil.GetUIPath(uiType.ToString());
        Transform root = GetRootTransform(uiRootType);

        GameObject uiInstance = await ResourceManager.Instance.InstantiateGameObjectAsync(path, root, cancellationToken: cancellationToken);

        if (uiInstance == null)
        {
            Debug.LogError($"[UIManager:CreateUI] 현재 인스턴스가 존재하여 중복 오브젝트를 파괴합니다.");
            return null;
        }

        if (!uiInstance.TryGetComponent(out UIBase uiBase))
        {
            Debug.LogError($"[UIManager:TryGetComponent] {uiType}에 현재 인스턴스의 컴포넌트를 찾을 수 없습니다.");
            return uiBase;
        }

        _createUIDictionary[uiType] = uiBase;
        return uiBase;
    }

    private async UniTask<UIBase> GetOrCreateUI(UIRootType uiRootType, UIType uiType, CancellationToken cancellationToken = default)
    {
        if (!_createUIDictionary.TryGetValue(uiType, out UIBase createUi))
        {
            createUi = await CreateUI(uiRootType, uiType, cancellationToken);
        }

        return createUi;
    }

    private Transform GetRootTransform(UIRootType uiRootType)
    {
        Transform root = null;
        switch (uiRootType)
        {
            case UIRootType.BackgroundUI:
                root = Canvas_BgRoot;
                break;
            case UIRootType.MainUI:
                root = Canvas_MainRoot;
                break;
            case UIRootType.ContentUI:
                root = Canvas_ContentRoot;
                break;
            case UIRootType.PopupUI:
                root = Canvas_PopupRoot;
                break;
            case UIRootType.VeryFrontUI:
                root = Canvas_VeryFrontRoot;
                break;
        }
        return root;
    }

    private async UniTask<UIBase> OpenUIAsync(UIRootType uiRootType, UIType uiType, bool isInitialHide, CancellationToken cancellationToken = default)
    {
        UIBase openedUI = await GetOrCreateUI(uiRootType, uiType, cancellationToken);

        if (openedUI == null)
        {
            Debug.LogWarning("[UIManager:GetOrCreateUI] 생성된 UI가 없습니다.");
            return null;
        }

        if (_openedUIDictionary.Contains(uiType) == false)
        {
            openedUI.gameObject.SetActive(true);
            _openedUIDictionary.Add(uiType);
        }
        return openedUI;
    }
    #endregion
}