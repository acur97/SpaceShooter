using UnityEngine;

public class StoreManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private RectTransform topPanel;
    [SerializeField] private RectTransform bottomPanel;

    private const float limit = 1.777777f;
    private float cameraAspect = 1.777778f;

    private void Update()
    {
        cameraAspect = cam.aspect;

        if (cameraAspect >= limit)
        {
            topPanel.sizeDelta = new Vector2(0, 78f);
            bottomPanel.offsetMax = new Vector2(0, -78f);
        }
        else
        {
            topPanel.sizeDelta = new Vector2(0, 152f);
            bottomPanel.offsetMax = new Vector2(0, -152f);
        }
    }
}