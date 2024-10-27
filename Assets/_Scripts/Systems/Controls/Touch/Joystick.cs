using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private Image backPanel;
    [SerializeField] private Image knob;

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

    public virtual void OnDrag(PointerEventData pointerEventData)
    {
        position = Vector2.zero;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(backPanel.rectTransform, pointerEventData.position, pointerEventData.pressEventCamera, out position))
        {
            sizeDelta = backPanel.rectTransform.sizeDelta;
            pivot = backPanel.rectTransform.pivot;

            // Get the touch position
            position.x /= sizeDelta.x;
            position.y /= sizeDelta.y;

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
            InputDirection = new Vector2(x, y);
            InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;

            // Move the knob
            knob.rectTransform.anchoredPosition = new Vector2(InputDirection.x * (backPanel.rectTransform.sizeDelta.x / 3), InputDirection.y * (backPanel.rectTransform.sizeDelta.y / 3));

            // More sensitive input
            InputDirection.x = Math.Clamp(InputDirection.x * 2, -1, 1);
            InputDirection.y = Math.Clamp(InputDirection.y * 2, -1, 1);
        }
    }

    public virtual void OnPointerDown(PointerEventData pointerEventData)
    {
        OnDrag(pointerEventData);
    }

    public virtual void OnPointerUp(PointerEventData pointerEventData)
    {
        InputDirection = Vector2.zero;
        knob.rectTransform.anchoredPosition = Vector2.zero;
    }
}