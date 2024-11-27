using UnityEngine;
using UnityEngine.EventSystems;

public class ControlsManager : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Editor controls")]
    [SerializeField] private bool editorTouch = false;
#endif

    [Header("Controls")]
    public bool fireDown;
    public bool fireUp;
    public Vector2 move;
    public bool power;

    [Header("Touch Controls")]
    [SerializeField] private bool touchControls;
    [SerializeField] private GameObject touchUi;
    [SerializeField] private EventTrigger fireBtn;
    [SerializeField] private EventTrigger powerBtn;
    [SerializeField] private Joystick joystick;

    private const string _Horizontal = "Horizontal";
    private const string _Vertical = "Vertical";
    private const string _Fire = "Fire1";
    private const string _Jump = "Jump";

    [Header("Tutorial")]
    [SerializeField] private GameObject tutorialMobile;
    [SerializeField] private GameObject tutorialPC;

    [System.Obsolete]
    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // Call the JavaScript function to check the device
        Application.ExternalCall("CheckDevice");
#elif UNITY_EDITOR
        OnDeviceCheck(editorTouch ? 1 : 0);
#endif
    }

    // This method will be called from JavaScript
    public void OnDeviceCheck(int isMobile)
    {
        touchControls = isMobile == 1;
        touchUi.SetActive(touchControls);

        if (touchControls)
        {
            // fire btn
            EventTrigger.Entry fireEntryDown = new()
            {
                eventID = EventTriggerType.PointerDown
            };
            fireEntryDown.callback.AddListener((eventData) => { fireDown = true; fireUp = false; });
            fireBtn.triggers.Add(fireEntryDown);

            EventTrigger.Entry fireEntryUp = new()
            {
                eventID = EventTriggerType.PointerUp
            };
            fireEntryUp.callback.AddListener((eventData) => { fireDown = false; fireUp = true; });
            fireBtn.triggers.Add(fireEntryUp);


            // power btn
            EventTrigger.Entry powerEntryDown = new()
            {
                eventID = EventTriggerType.PointerDown
            };
            powerEntryDown.callback.AddListener((eventData) => power = true);
            powerBtn.triggers.Add(powerEntryDown);

            EventTrigger.Entry powerEntryUp = new()
            {
                eventID = EventTriggerType.PointerUp
            };
            powerEntryUp.callback.AddListener((eventData) => power = false);
            powerBtn.triggers.Add(powerEntryUp);
        }
    }

    public void OpenTutorial(bool on)
    {
        if (on)
        {
            if (touchControls)
            {
                tutorialMobile.SetActive(true);
                tutorialPC.SetActive(false);
            }
            else
            {
                tutorialMobile.SetActive(false);
                tutorialPC.SetActive(true);
            }
        }

        UiManager.Instance.SetUi(UiType.Tutorial, on, 0.5f);
        UiManager.Instance.SetUi(UiType.Select, !on, 0.5f);
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

            if (power == true)
                power = false;
        }
        else
        {
            //switch (RoundsController.Instance.levelType)
            //{
            //    case RoundsController.LevelType.Normal:
            //        move.x = Input.GetAxis(_Horizontal);
            //        move.y = Input.GetAxis(_Vertical);
            //        break;

            //    case RoundsController.LevelType.Inifinite:
                    move.x = Input.GetAxisRaw(_Horizontal);
                    move.y = Input.GetAxisRaw(_Vertical);
                    //break;
            //}

            fireDown = Input.GetButtonDown(_Fire);
            fireUp = Input.GetButtonUp(_Fire);

            power = Input.GetButtonDown(_Jump);
        }
    }
}