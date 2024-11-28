using UnityEngine;
using UnityEngine.Events;

public class OnInvoke : MonoBehaviour
{
    public UnityEvent OnInvokeEvent;
    public UnityEvent OnEnableEvent;
    public UnityEvent OnCancelEvent;

    public void Invoke()
    {
        OnInvokeEvent.Invoke();
    }

    private void OnEnable()
    {
        OnEnableEvent.Invoke();
    }

    private void Update()
    {
        if (Input.GetButtonDown(Inputs.Cancel))
        {
            OnCancelEvent.Invoke();
        }
    }
}