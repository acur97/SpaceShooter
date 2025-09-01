using System;
using UnityEngine;

//[ExecuteInEditMode]
public class RectSafeArea : MonoBehaviour
{
    [SerializeField] private RectTransform parentRectTransform;
    [SerializeField] private RectTransform rectTransform;

#if Platform_Mobile || UNITY_EDITOR

    public static Action<bool, float> RefreshAdSafeArea;

    private Rect lastSafeArea;
    private Rect safeAreaRect;
    private float scaleRatio;
    private float left;
    private float right;
    private float top;
    public static float bottom;
    private bool bottomAd = false;
    private float adSpace = 0f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        RefreshAdSafeArea = null;
        bottom = 0;
    }

    private void OnEnable()
    {
        RefreshAdSafeArea += InitAdSpace;
    }

    private void OnDestroy()
    {
        RefreshAdSafeArea -= InitAdSpace;
    }

    private void InitAdSpace(bool addOn, float _adSpace)
    {
        bottomAd = addOn;
        adSpace = addOn ? _adSpace : 0f;
        ApplySafeArea();
    }

    private void Update()
    {
        if (lastSafeArea != Screen.safeArea)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        safeAreaRect = Screen.safeArea;

        scaleRatio = parentRectTransform.rect.width / Screen.width;

        left = safeAreaRect.xMin * scaleRatio;
        right = -(Screen.width - safeAreaRect.xMax) * scaleRatio;
        bottom = safeAreaRect.yMin * scaleRatio;
        if (bottomAd)
        {
            bottom += adSpace - bottom;
        }
        top = -(Screen.height - safeAreaRect.yMax) * scaleRatio;

        rectTransform.offsetMin = new Vector2(left, bottom);
        rectTransform.offsetMax = new Vector2(right, top);

        lastSafeArea = Screen.safeArea;
    }

#endif
}