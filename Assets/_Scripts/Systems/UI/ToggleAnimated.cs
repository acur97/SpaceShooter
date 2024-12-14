using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleAnimated : Toggle
{
    [SerializeField] private float scale = 1.1f;

    private Vector3 initialScale;

    protected override void Awake()
    {
        Debug.LogWarning("Awake");

        base.Awake();

        initialScale = transform.localScale;
    }

    public override void Select()
    {
        Debug.LogWarning("Select");

        base.Select();
        Scale();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        Debug.LogWarning("OnSelect");

        base.OnSelect(eventData);
        Scale();
    }

    private void Scale()
    {
        Debug.LogWarning("Scale");

        transform.localScale = initialScale * scale;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        Debug.LogWarning("OnDeselect");

        base.OnDeselect(eventData);
        transform.localScale = initialScale;
    }
}