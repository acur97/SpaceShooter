using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private Image backPanel;
    [SerializeField] private Image knob;
    [SerializeField] private float multiplier = 2f;
    [SerializeField] private float knobClamp = 4f;
    [SerializeField] private float knobOffset = 1f;

    public Vector2 InputDirection;

    private Vector2 position;
    private Vector2 sizeDelta;
    private Vector2 pivot;
    private float x;
    private float y;

    private void Awake()
    {
        OnPointerUp(null);
    }

    public virtual void OnPointerDown(PointerEventData pointerEventData)
    {
        OnDrag(pointerEventData);
    }

    public virtual void OnDrag(PointerEventData pointerEventData)
    {
        position = backPanel.rectTransform.InverseTransformPoint(pointerEventData.pointerCurrentRaycast.worldPosition);

        sizeDelta = backPanel.rectTransform.sizeDelta;
        pivot = backPanel.rectTransform.pivot;

        // Get the touch position
        position /= sizeDelta;

        // Calculate the move position
        x = pivot.x switch
        {
            0 => position.x * 2 - 1,
            1 => position.x * 2 + 1,
            _ => position.x * 2
        };
        y = pivot.y switch
        {
            0 => position.y * 2 - 1,
            1 => position.y * 2 + 1,
            _ => position.y * 2
        };

        // Get the input position
        InputDirection.x = x;
        InputDirection.y = y;
        InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;

        // Move the knob
        knob.rectTransform.anchoredPosition = Vector2.ClampMagnitude(InputDirection * (backPanel.rectTransform.sizeDelta / 3) * knobOffset, knobClamp);

        // More sensitive input
        InputDirection.x = Math.Clamp(InputDirection.x * multiplier, -1, 1);
        InputDirection.y = Math.Clamp(InputDirection.y * multiplier, -1, 1);
    }

    public virtual void OnPointerUp(PointerEventData _)
    {
        InputDirection = Vector2.zero;
        knob.rectTransform.anchoredPosition = Vector2.zero;
    }
}