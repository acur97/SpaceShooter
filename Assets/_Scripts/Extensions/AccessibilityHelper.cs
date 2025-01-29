using UnityEngine;

public class AccessibilityHelper : MonoBehaviour
{
    public string contentLabel;

#if UNITY_ANDROID && !UNITY_EDITOR
    void Start()
    {
        if (!string.IsNullOrEmpty(contentLabel))
        {
            SetContentDescription(gameObject, contentLabel);
        }
    }

    private void SetContentDescription(GameObject obj, string description)
    {
        using AndroidJavaClass unityPlayer = new("com.unity3d.player.UnityPlayer");
        using AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        using AndroidJavaObject view = activity.Call<AndroidJavaObject>("findViewById", obj.GetInstanceID());
        view?.Call("setContentDescription", description);
    }
#endif
}