using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    [Space]
    [SerializeField] private CanvasGroup canvasGameplay;
    [SerializeField] private CanvasGroup canvasPause;
    [SerializeField] private CanvasGroup canvasSelect;
    [SerializeField] private CanvasGroup canvasStore;
    [SerializeField] private CanvasGroup canvasWnew;
    [SerializeField] private CanvasGroup canvasTutorial;
    [SerializeField] private CanvasGroup canvasEnd;
    [SerializeField] private CanvasGroup canvasLeaderboard;
    [SerializeField] private CanvasGroup canvasFade;
    [SerializeField] private CanvasGroup canvasLoading;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Instance = null;
    }

    public void Init()
    {
        Instance = this;

        canvasGameplay.gameObject.SetActive(false);
        canvasPause.gameObject.SetActive(false);
        canvasSelect.gameObject.SetActive(false);
        canvasStore.gameObject.SetActive(false);
        canvasWnew.gameObject.SetActive(false);
        canvasTutorial.gameObject.SetActive(false);
        canvasEnd.gameObject.SetActive(false);
        canvasLeaderboard.gameObject.SetActive(false);
        canvasFade.gameObject.SetActive(false);
        canvasLoading.gameObject.SetActive(false);

        SetUi(UiType.Fade, true);
    }

#pragma warning disable UNT0006, IDE0051 // Incorrect message signature
    private async UniTaskVoid Start()
    {
        await UniTask.DelayFrame(1);

        FadeCanvas(canvasFade, false, 1);
        SetUi(UiType.Select, true);
    }
#pragma warning restore IDE0051, UNT0006 // Incorrect message signature

    public void SetUi(UiType type, bool active, float fadeTime = -1f, Action callback = null)
    {

        switch (type)
        {
            case UiType.Gameplay:
                if (fadeTime > 0)
                    FadeCanvas(canvasGameplay, active, fadeTime, callback);
                else
                    canvasGameplay.gameObject.SetActive(active);
                break;

            case UiType.Pause:
                if (fadeTime > 0)
                    FadeCanvas(canvasPause, active, fadeTime, callback);
                else
                    canvasPause.gameObject.SetActive(active);
                break;

            case UiType.Select:
                if (fadeTime > 0)
                    FadeCanvas(canvasSelect, active, fadeTime, callback);
                else
                    canvasSelect.gameObject.SetActive(active);
                break;

            case UiType.Store:
                if (fadeTime > 0)
                    FadeCanvas(canvasStore, active, fadeTime, callback);
                else
                    canvasStore.gameObject.SetActive(active);
                break;

            case UiType.Wnew:
                if (fadeTime > 0)
                    FadeCanvas(canvasWnew, active, fadeTime, callback);
                else
                    canvasWnew.gameObject.SetActive(active);
                break;

            case UiType.Tutorial:
                if (fadeTime > 0)
                    FadeCanvas(canvasTutorial, active, fadeTime, callback);
                else
                    canvasTutorial.gameObject.SetActive(active);
                break;

            case UiType.End:
                if (fadeTime > 0)
                    FadeCanvas(canvasEnd, active, fadeTime, callback);
                else
                    canvasEnd.gameObject.SetActive(active);
                break;

            case UiType.Leaderboard:
                if (fadeTime > 0)
                    FadeCanvas(canvasLeaderboard, active, fadeTime, callback);
                else
                    canvasLeaderboard.gameObject.SetActive(active);
                break;

            case UiType.Fade:
                if (fadeTime > 0)
                    FadeCanvas(canvasFade, active, fadeTime, callback);
                else
                    canvasFade.gameObject.SetActive(active);
                break;

            case UiType.Loading:
                if (fadeTime > 0)
                    FadeCanvas(canvasLoading, active, fadeTime, callback);
                else
                    canvasLoading.gameObject.SetActive(active);
                break;
        }
    }

    private void FadeCanvas(CanvasGroup canvas, bool active, float duration, Action callback = null)
    {
        canvas.interactable = active;
        canvas.alpha = active ? 0 : 1;

        canvas.gameObject.SetActive(canvas.gameObject.activeSelf || true);

        LeanTween.alphaCanvas(canvas, active ? 1 : 0, duration)
            .setOnComplete(OnFadeComplete)
            .setOnCompleteParam(new FadeData { Canvas = canvas, Active = active, Callback = callback })
            .setIgnoreTimeScale(true);
    }

    private class FadeData
    {
        public CanvasGroup Canvas;
        public bool Active;
        public Action Callback;
    }

    private static void OnFadeComplete(object param)
    {
        FadeData data = (FadeData)param;
        data.Callback?.Invoke();
        data.Canvas.gameObject.SetActive(data.Active);
    }
}

public enum UiType
{
    None,
    Gameplay,
    Pause,
    Select,
    Store,
    Wnew,
    Tutorial,
    End,
    Leaderboard,
    Fade,
    Loading
}