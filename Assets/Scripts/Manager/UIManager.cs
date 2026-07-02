using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

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
  
    public async UniTask<UIBase> OpenMainUIAsync(UIType uiType, CancellationToken cancellationToken = default)
    {
        UIBase uIBase = await OpenUIAsync(UIRootType.MainUI, uiType, cancellationToken);
        return uIBase;
    }

    public async UniTask<UIBase> OpenContentUIAsync(UIType uiType, CancellationToken cancellationToken = default)
    {
        UIBase uIBase = await OpenUIAsync(UIRootType.ContentUI, uiType, cancellationToken);
        return uIBase;
    }

    public async UniTask<UIBase> OpenPopupUIAsync(UIType uiType, CancellationToken cancellationToken = default)
    {
        UIBase uIBase = await OpenUIAsync(UIRootType.PopupUI, uiType, cancellationToken);
        return uIBase;
    }

    public async UniTask<UIBase> OpenVeryFrontUIAsync(UIType uiType, CancellationToken cancellationToken = default)
    {
        UIBase uIBase = await OpenUIAsync(UIRootType.VeryFrontUI, uiType, cancellationToken);
        return uIBase;
    }

    public async UniTask OpenFadeUI(Action onComplete = null, CancellationToken cancellationToken = default)
    {
        var uiBase = await OpenVeryFrontUIAsync(UIType.FadePopupUI, cancellationToken: cancellationToken);

        if (uiBase is FadePopupUI fadePopupUI)
        {
            await fadePopupUI.Fade(onComplete);
        }
    }

    //수정
    public void CloseUI(UIType uiType)
    {
        if (!_openedUIDictionary.Contains(uiType))
        {
            Debug.LogError("[UIManager:CloseUI] 열려있지 않은 UI 입니다.");
            return;
        }

        UIBase openUi = _createUIDictionary[uiType];

        if (openUi == null)
        {
            _createUIDictionary.Remove(uiType);
        }

        openUi.gameObject.SetActive(false);
        _openedUIDictionary.Remove(uiType);
    }

    #region 메인 로직
    private async UniTask<UIBase> CreateUI(UIRootType uiRootType, UIType uiType, CancellationToken cancellationToken = default)
    {
        string path = AddressableUtil.GetUIPath(uiType.ToString());
        Transform root = GetRootTransform(uiRootType);

        GameObject uiInstance = await ResourceManager.Instance.InstantiateGameObjectAsync(path, root, cancellationToken: cancellationToken);

        if (uiInstance == null)
        {
            Debug.LogError($"[UIManager:CreateUI] UI를 생성하지 못했습니다.");
            return null;
        }

        if (!uiInstance.TryGetComponent(out UIBase uiBase))
        {
            Debug.LogError($"[UIManager:CreateUI] UIBase 컴포넌트를 찾을 수 없습니다.");
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

    private async UniTask<UIBase> OpenUIAsync(UIRootType uiRootType, UIType uiType, CancellationToken cancellationToken = default)
    {
        UIBase openedUI = await GetOrCreateUI(uiRootType, uiType, cancellationToken);

        if (openedUI == null)
        {
            Debug.LogWarning("[UIManager:OpenUIAsync] 생성된 UI가 없습니다.");
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