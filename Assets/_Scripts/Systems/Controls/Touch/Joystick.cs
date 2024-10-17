using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private Image backPanel;
    [SerializeField] private Image knob;

    public Vector2 InputDirection;

    private Vector2 position;

    private void Awake()
    {
        OnPointerUp(null);
    }

    public virtual void OnDrag(PointerEventData pointerEventData)
    {
        position = Vector2.zero;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(backPanel.rectTransform, pointerEventData.position, pointerEventData.pressEventCamera, out position))
        {
            // Get the touch position
            position.x = (position.x / backPanel.rectTransform.sizeDelta.x);
            position.y = (position.y / backPanel.rectTransform.sizeDelta.y);

            // Calculate the move position
            float x = (backPanel.rectTransform.pivot.x == 1) ? position.x * 2 + 1 : position.x * 2 - 1;
            float y = (backPanel.rectTransform.pivot.y == 1) ? position.y * 2 + 1 : position.y * 2 - 1;

            // Get the input position
            InputDirection = new Vector2(x, y);
            InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;

            // Move the knob
            knob.rectTransform.anchoredPosition = new Vector2(InputDirection.x * (backPanel.rectTransform.sizeDelta.x / 3), InputDirection.y * (backPanel.rectTransform.sizeDelta.y / 3));
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