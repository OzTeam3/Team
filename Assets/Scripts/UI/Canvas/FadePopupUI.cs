using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class FadePopupUI : UIBase
{
    [SerializeField] private Image _backGround;
    [SerializeField] private Image _fade;

    [SerializeField] private float _fadeTime = 1f;

    private CancellationTokenSource _disableCancellationToken;
   
    private void OnEnable()
    {
        _disableCancellationToken = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        if (_disableCancellationToken == null)
        {
            return;
        }

        _disableCancellationToken.Cancel();
        _disableCancellationToken.Dispose();
        _disableCancellationToken = null;
    }

    public async UniTask Fade(Action onComplete = null)
    {
        _backGround.gameObject.SetActive(true);

        await FadeIn();

        onComplete?.Invoke();

        _fade.gameObject.SetActive(false);

        await UniTask.WaitForSeconds(1f, cancellationToken: _disableCancellationToken.Token);
        await FadeOut();
    }

    private async UniTask FadeIn()
    {
        float time = 0f;
        Color alpha = _backGround.color;
        alpha.a = 0f;
        _backGround.color = alpha;

        while (alpha.a < 1f)
        {
            time += Time.deltaTime / _fadeTime;
            alpha.a = Mathf.Lerp(0, 1, time);
            _backGround.color = alpha;
            await UniTask.Yield(_disableCancellationToken.Token);
        }
    }

    private async UniTask FadeOut()
    {
        float time = 0f;
        Color alpha = _backGround.color;

        while (alpha.a > 0f)
        {
            time += Time.deltaTime / _fadeTime;
            alpha.a = Mathf.Lerp(1, 0, time);
            _backGround.color = alpha;
            await UniTask.Yield(_disableCancellationToken.Token);
        }
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        UIManager.Instance.CloseUI(UIType.FadePopupUI);
    }
}