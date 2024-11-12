using System;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    public static Action onRefresh;

    [Header("Store")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform content;

    [Header("UI")]
    [SerializeField] private Camera cam;
    [SerializeField] private RectTransform topPanel;
    [SerializeField] private RectTransform bottomPanel;

    private const float limit = 1.777777f;
    private float cameraAspect = 1.777778f;

    private void Awake()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        for (int i = 0; i < PowerUpsManager.Instance.powerUps.Count; i++)
        {
            Instantiate(prefab, content).GetComponent<StoreItem>().Init(PowerUpsManager.Instance.powerUps[i]);
        }
    }

    private void Start()
    {
        onRefresh?.Invoke();
    }

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