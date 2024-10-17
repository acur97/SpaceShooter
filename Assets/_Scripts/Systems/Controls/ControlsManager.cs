using UnityEngine;
using UnityEngine.EventSystems;

public class ControlsManager : MonoBehaviour
{
    [Header("Controls")]
    public bool fireDown;
    public bool fireUp;
    public Vector2 move;

    [Header("Touch Controls")]
    [SerializeField] private bool touchControls;
    [SerializeField] private GameObject touchUi;
    [SerializeField] private EventTrigger fireBtn;
    [SerializeField] private Joystick joystick;

    private const string _Horizontal = "Horizontal";
    private const string _Vertical = "Vertical";
    private const string _Fire = "Fire1";

    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // Call the JavaScript function to check the device
        Application.ExternalCall("CheckDevice");
#elif UNITY_EDITOR
        OnDeviceCheck(true);
#endif
    }

    // This method will be called from JavaScript
    public void OnDeviceCheck(bool isPc)
    {
        touchControls = !isPc;
        touchUi.SetActive(!isPc);

        if (!isPc)
        {
            EventTrigger.Entry entryDown = new()
            {
                eventID = EventTriggerType.PointerDown
            };
            entryDown.callback.AddListener((eventData) => { fireDown = true; fireUp = false; });
            fireBtn.triggers.Add(entryDown);

            EventTrigger.Entry entryUp = new()
            {
                eventID = EventTriggerType.PointerUp
            };
            entryUp.callback.AddListener((eventData) => { fireDown = false; fireUp = true; });
            fireBtn.triggers.Add(entryUp);
        }
    }

    private void LateUpdate()
    {
        if (touchControls)
        {
            move = joystick.InputDirection;
            if (fireDown == true)
                fireDown = false;
            if (fireUp == true)
                fireUp = false;
        }
        else
        {
            move = new Vector2(Input.GetAxis(_Horizontal), Input.GetAxis(_Vertical));
            fireDown = Input.GetButtonDown(_Fire);
            fireUp = Input.GetButtonUp(_Fire);
        }
    }
}