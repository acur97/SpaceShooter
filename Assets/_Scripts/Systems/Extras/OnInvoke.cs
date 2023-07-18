using UnityEngine;
using UnityEngine.Events;

public class OnInvoke : MonoBehaviour
{
    public UnityEvent OnInvokeEvent;

    public void Invoke()
    {
        OnInvokeEvent.Invoke();
    }
}