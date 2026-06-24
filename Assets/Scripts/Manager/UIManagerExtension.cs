using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum UIRootType
{
    None = 0,
    BackgroundUI,
    MainUI,
    ContentUI,
    PopupUI,
    VeryFrontUI
}

public enum UIType
{
    OpeningUI,
    StartPopupUI,
    MainHUD,
    FadePopupUI
}

public static class UIManagerExtension
{
    public static string GetUIPath(this UIManager uiManager, UIRootType uiRootType, UIType uiType)
    {
        return $"Assets/Prefabs/UI/{uiType}";
    }

    public static void ShowStartupUIOnGameStart(this UIManager uiManager)
    {
        uiManager.OpenFadeUI(OnFadeComplete).Forget();
    }

    public static void OnFadeComplete()
    {
        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.OpeningUI);
        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.MainHUD);
    }

    //Addressables는 비동기 로드이므로, 로드 완료 후 인스턴스를 받아 Fade()를 호출하기 위해 UniTask로 작성
    public static async UniTask OpenFadeUI(this UIManager uiManager, Action onComplete = null)
    {
        var uiBase = await uiManager.OpenPopupUIAsync(UIType.FadePopupUI);
        if (uiBase is FadePopupUI fadePopupUI)
        {
            fadePopupUI.Fade(onComplete);
        }
    }
}
