using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleScaled : Toggle
{
    private const float scale = 1.02f;

    private Vector3 initialScale;

    protected override void Awake()
    {
        base.Awake();

        initialScale = transform.localScale;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        Scale();
    }

    //public override void Select()
    //{
    //    base.Select();
    //    Scale();
    //}

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        Scale();
    }

    private void Scale()
    {
        transform.localScale = initialScale * scale;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        transform.localScale = initialScale;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        transform.localScale = initialScale;
    }
}