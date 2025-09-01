using UnityEngine;
using UnityEngine.EventSystems;

public class ControlsManager : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Editor controls")]
    [SerializeField] private bool editorMobile = false;
#endif

    [Header("Controls")]
    [ReadOnly] public bool fireDown;
    [ReadOnly] public bool fireUp;
    [ReadOnly] public Vector2 move;
    [ReadOnly] public bool power;

    [Header("Touch Controls")]
    public static bool hasTouch;
    [SerializeField] private GameObject touchUi;
    public EventTrigger powerBtn;
    [SerializeField] private Swipe swipe;

    [Header("Tutorial")]
    [SerializeField] private GameObject tutorialMobile;
    [SerializeField] private GameObject tutorialPC;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        hasTouch = false;
    }

    private void OnEnable()
    {
        GameManager.GamePreStart += SetLevelTypeBtns;
    }

    [System.Obsolete]
    private void Start()
    {
#if Platform_Web && !UNITY_EDITOR
        Application.ExternalCall("CheckDevice");
#elif UNITY_EDITOR
        OnDeviceCheck(editorMobile ? 1 : 0);

#elif Platform_Mobile
        OnDeviceCheck(1);
#endif
    }

    private void SetLevelTypeBtns()
    {
        switch (RoundsController.Instance.levelType)
        {
            case RoundsController.LevelType.Normal:
                powerBtn.gameObject.SetActive(GameManager.Instance.gameplayScriptable.selectedPowerUp != null);
                break;

            case RoundsController.LevelType.Infinite:
                powerBtn.gameObject.SetActive(GameManager.Instance.gameplayScriptable.selectedPowerUp != null);
                break;
        }
    }

    // This method will be called from JavaScript for WebGl builds
    public void OnDeviceCheck(int isMobile)
    {
        hasTouch = isMobile == 1;
        touchUi.SetActive(hasTouch);

#if Platform_Web
        GameManager.Instance.SetQR(hasTouch);
#endif

        if (hasTouch)
        {
            // fire btn
            swipe.OnTouch += OnTouch;

            // power btn
            EventTrigger.Entry powerEntryDown = new()
            {
                eventID = EventTriggerType.PointerDown
            };
            powerEntryDown.callback.AddListener(OnPowerTrue);
            powerBtn.triggers.Add(powerEntryDown);

            EventTrigger.Entry powerEntryUp = new()
            {
                eventID = EventTriggerType.PointerUp
            };
            powerEntryUp.callback.AddListener(OnPowerFalse);
            powerBtn.triggers.Add(powerEntryUp);
        }
    }

    private void OnDestroy()
    {
        GameManager.GamePreStart -= SetLevelTypeBtns;
        swipe.OnTouch -= OnTouch;
    }

    private void OnTouch(bool value)
    {
        fireDown = value;
        fireUp = !value;
    }

    private void OnPowerTrue(BaseEventData _)
    {
        power = true;
    }

    private void OnPowerFalse(BaseEventData _)
    {
        power = false;
    }

    public void OpenTutorial(bool on)
    {
        if (on)
        {
            if (hasTouch)
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
        if (hasTouch)
        {
            move = swipe.InputDirection;
            swipe.InputDirection = Vector2.zero;

            if (fireDown == true)
                fireDown = false;

            if (fireUp == true)
                fireUp = false;

            if (power == true)
                power = false;
        }
        else
        {
            move.x = Input.GetAxisRaw(Inputs.Horizontal);
            move.y = Input.GetAxisRaw(Inputs.Vertical);

            fireDown = Input.GetButtonDown(Inputs.Fire);
            fireUp = Input.GetButtonUp(Inputs.Fire);

            power = Input.GetButtonDown(Inputs.Jump);
        }
    }
}