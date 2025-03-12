using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Swipe : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public Action<bool> OnTouch;

    [ReadOnly] public Vector2 InputDirection = Vector2.zero;

    [Space]
    [SerializeField] private float multiplier = 16;

    private Vector2 dragPosition;

    private void Awake()
    {
        OnPointerUp(null);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);

        OnTouch?.Invoke(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragPosition = eventData.delta;

        InputDirection = 1280f / Screen.width * multiplier * dragPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputDirection = Vector2.zero;

        OnTouch?.Invoke(false);
    }
}