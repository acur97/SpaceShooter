using UnityEngine;
using UnityEngine.EventSystems;

public class PreventDeselection : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;

    private GameObject lastSelected;

    private void Awake()
    {
        lastSelected = eventSystem.firstSelectedGameObject;
    }

    private void Update()
    {
        if (eventSystem.currentSelectedGameObject == null)
        {
            eventSystem.SetSelectedGameObject(lastSelected);
            return;
        }

        lastSelected = eventSystem.currentSelectedGameObject;
    }
}