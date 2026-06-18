using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas Canvas_BgRoot;
    [SerializeField] private Canvas Canvas_MainRoot;
    [SerializeField] private Canvas Canvas_ContentRoot;
    [SerializeField] private Canvas Canvas_PopupRoot;
    [SerializeField] private Canvas Canvas_VeryFrontRoot;

    private Dictionary<UIType, UIBase> _createrdUIDictionary = new Dictionary<UIType, UIBase>();
    private Dictionary<UIType, UniTask<UIBase>> _loadingTaskDictionary = new Dictionary<UIType, UniTask<UIBase>>();
    private HashSet<UIType> _openedUIDictionary = new HashSet<UIType>();

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        this.ShowStartupUIOnGameStart();
    }

    //프리팹을 경로로 가져와서 인스턴스화 하는 함수
    private async UniTask<UIBase> CreateUI(UIRootType uiRootType, UIType uiType)
    {
        string path = this.GetUIPath(uiRootType, uiType);
        Transform root = GetRootTransform(uiRootType);

        AsyncOperationHandle<GameObject> loadHandle = Addressables.LoadAssetAsync<GameObject>(path);
        await loadHandle.ToUniTask();

        GameObject prefab = loadHandle.Result;
        if (prefab == null)
        {
            Debug.LogError($"[UIManager] UI 로드 실패: {uiType} ({path})");
            _loadingTaskDictionary.Remove(uiType);
            return null;
        }

        GameObject uiInstance = Instantiate(prefab, root);
        UIBase uiBase = uiInstance.GetComponent<UIBase>();
        _createrdUIDictionary[uiType] = uiBase;
        _loadingTaskDictionary.Remove(uiType);
        return uiBase;
    }

    // 이미 만들어져 있으면 즉시 반환, 로딩 중이면 그 작업을 같이 기다림, 둘 다 아니면 새로 로드 시작
    private UniTask<UIBase> GetOrCreateUI(UIRootType uiRootType, UIType uiType)
    {
        if (_createrdUIDictionary.TryGetValue(uiType, out UIBase existingUI))
        {
            return UniTask.FromResult(existingUI);
        }

        if (_loadingTaskDictionary.TryGetValue(uiType, out UniTask<UIBase> ongoingTask))
        {
            return ongoingTask;
        }

        UniTask<UIBase> newTask = CreateUI(uiRootType, uiType);
        _loadingTaskDictionary[uiType] = newTask;
        return newTask;
    }

    private Transform GetRootTransform(UIRootType uiRootType)
    {
        Transform root = null;
        switch (uiRootType)
        {
            case UIRootType.BackgroundUI:
                root = Canvas_BgRoot.transform;
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

    private async UniTask<UIBase> OpenUIAsync(UIRootType uiRootType, UIType uiType, bool isInitialHide)
    {
        UIBase openedUI = await GetOrCreateUI(uiRootType, uiType);
        if (openedUI == null) return null;

        if (_openedUIDictionary.Contains(uiType) == false)
        {
            openedUI.gameObject.SetActive(isInitialHide == false);
            _openedUIDictionary.Add(uiType);
        }
        return openedUI;
    }

    public void OpenUI(UIRootType uiRootType, UIType uiType, bool isInitialHide = false)
    {
        OpenUIAsync(uiRootType, uiType, isInitialHide).Forget();
    }

    public void CloseUI(UIRootType uiRootType, UIType uiType)
    {
        if (_openedUIDictionary.Contains(uiType))
        {
            var openedUi = _createrdUIDictionary[uiType];
            openedUi.gameObject.SetActive(false);
            _openedUIDictionary.Remove(uiType);
        }
    }

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
}