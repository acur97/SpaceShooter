using UnityEngine;
using UnityEngine.Events;

public class OnInvoke : MonoBehaviour
{
    public UnityEvent OnInvokeEvent;
    public UnityEvent OnEnableEvent;
    public UnityEvent OnDisableEvent;
    public UnityEvent OnCancelEvent;

    public void Invoke()
    {
        OnInvokeEvent.Invoke();
    }

    private void OnEnable()
    {
        OnEnableEvent.Invoke();
    }

    private void OnDisable()
    {
        OnDisableEvent.Invoke();
    }

    private void Update()
    {
        if (Input.GetButtonDown(Inputs.Cancel) && !PopupManager.Instance.isOpen)
        {
            OnCancelEvent.Invoke();
        }
    }
}