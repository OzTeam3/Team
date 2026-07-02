using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class FadePopupUI : UIBase
{
    [SerializeField] private Image _imagePanel;
    [SerializeField] private Image _imagePane2;


    [SerializeField] private float _fadeTime = 1f;

    public async UniTask Fade(Action onComplete = null)
    {
        _imagePanel.gameObject.SetActive(true);

        await FadeIn();

        onComplete?.Invoke();

        _imagePane2.gameObject.SetActive(false);

        await UniTask.WaitForSeconds(1f);
        await FadeOut();
    }

    private async UniTask FadeIn()
    {
        float time = 0f;
        Color alpha = _imagePanel.color;
        alpha.a = 0f;
        _imagePanel.color = alpha;

        while (alpha.a < 1f)
        {
            time += Time.deltaTime / _fadeTime;
            alpha.a = Mathf.Lerp(0, 1, time);
            _imagePanel.color = alpha;
            await UniTask.Yield();
        }
    }

    private async UniTask FadeOut()
    {
        float time = 0f;
        Color alpha = _imagePanel.color;

        while (alpha.a > 0f)
        {
            time += Time.deltaTime / _fadeTime;
            alpha.a = Mathf.Lerp(1, 0, time);
            _imagePanel.color = alpha;
            await UniTask.Yield();
        }
    }
}