using UnityEngine;

//[ExecuteInEditMode]
public class RectSafeArea : MonoBehaviour
{
    [SerializeField] private RectTransform parentRectTransform;
    [SerializeField] private RectTransform rectTransform;

    private Rect lastSafeArea;
    private Rect safeAreaRect;
    private float scaleRatio;
    private float left;
    private float right;
    private float top;
    private float bottom;

#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
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
        top = -(Screen.height - safeAreaRect.yMax) * scaleRatio;

        rectTransform.offsetMin = new Vector2(left, bottom);
        rectTransform.offsetMax = new Vector2(right, top);

        lastSafeArea = Screen.safeArea;
    }
#endif
}