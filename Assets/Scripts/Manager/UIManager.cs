using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private RectTransform Canvas_BgRoot;
    [SerializeField] private Canvas Canvas_MainRoot;
    [SerializeField] private Canvas Canvas_ContentRoot;
    [SerializeField] private Canvas Canvas_PopupRoot;
    [SerializeField] private Canvas Canvas_VeryFrontRoot;

    private readonly Dictionary<UIType, UIBase> _createrdUIDictionary = new Dictionary<UIType, UIBase>();
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

    private void Start()
    {
        //이거 게임매니저로 가야됨
       // ShowStartupUIOnGameStart();
    }

    //유니태스크로 만드세요
    public void OpenUI(UIRootType uiRootType, UIType uiType, bool isInitialHide = false)
    {
        OpenUIAsync(uiRootType, uiType, isInitialHide).Forget();
    }

    //얼리리턴 예외처리 하세요
    public void CloseUI(UIRootType uiRootType, UIType uiType)
    {
        if (_openedUIDictionary.Contains(uiType))
        {
            var openedUi = _createrdUIDictionary[uiType];
            openedUi.gameObject.SetActive(false);
            _openedUIDictionary.Remove(uiType);
        }
    }

    //유니태스크로 바꾸기, 캔슬토큰
    public void OpenContentUI(UIType uiType)
    {
        OpenUI(UIRootType.ContentUI, uiType);
    }

    public void OpenPopupUI(UIType uiType)
    {
        OpenUI(UIRootType.PopupUI, uiType);
    }

    public void CloseContentUI(UIType uiType)
    {
        CloseUI(UIRootType.ContentUI, uiType);
    }

    public void ClosePopupUI(UIType uiType)
    {
        CloseUI(UIRootType.PopupUI, uiType);
    }

    //바꿔주세요
    public UniTask<UIBase> OpenPopupUIAsync(UIType uiType)
    {
        return OpenUIAsync(UIRootType.PopupUI, uiType, false);
    }

    //사용처를 봐야됨?
    public void PreloadUI(UIRootType uiRootType, UIType uiType)
    {
        GetOrCreateUI(uiRootType, uiType).Forget();
    }


    public async UniTask OpenFadeUI(Action onComplete = null)
    {
        var uiBase = await OpenPopupUIAsync(UIType.FadePopupUI);
        if (uiBase is FadePopupUI fadePopupUI)
        {
            fadePopupUI.Fade(onComplete);
        }
    }

    #region 메인 로직
    private async UniTask<UIBase> CreateUI(UIRootType uiRootType, UIType uiType)
    {
        string path = GameUtil.GetUIPath(uiType);
        Transform root = GetRootTransform(uiRootType);

        //캔슬토큰
        GameObject uiInstance = await ResourceManager.Instance.InstantiateGameObjectAsync(path, root);

        //에러 코드 리소스 매니저와 일치
        if (uiInstance == null)
        {
            Debug.LogError($"[UIManager] UI 로드 실패: {uiType} ({path})");
            return null;
        }

        UIBase uiBase = uiInstance.GetComponent<UIBase>();

        //out 이 먼가 params<< in<< 이 먼가
        //널체크 (trygetComponent)
        _createrdUIDictionary[uiType] = uiBase;
        return uiBase;
    }

    //작업하세요
    private UniTask<UIBase> GetOrCreateUI(UIRootType uiRootType, UIType uiType)
    {
        if (!_createrdUIDictionary.TryGetValue(uiType, out UIBase existingUI))
        {
            //딕셔너리에 가져온 값 사용
            Debug.Log("딕셔너리에서 가져와야됨");
        }

        UniTask<UIBase> newTask = CreateUI(uiRootType, uiType);
        return newTask;
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
                root = Canvas_MainRoot.transform;
                break;
            case UIRootType.ContentUI:
                root = Canvas_ContentRoot.transform;
                break;
            case UIRootType.PopupUI:
                root = Canvas_PopupRoot.transform;
                break;
            case UIRootType.VeryFrontUI:
                root = Canvas_VeryFrontRoot.transform;
                break;
        }
        return root;
    }

    //흐름 수정? 열린 유아이먄 리턴, 안열렸으면 열어야된다.
    private async UniTask<UIBase> OpenUIAsync(UIRootType uiRootType, UIType uiType, bool isInitialHide)
    {
        UIBase openedUI = await GetOrCreateUI(uiRootType, uiType);

        //에러체크
        if (openedUI == null) 
        {
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