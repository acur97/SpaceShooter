#if Platform_Web /// Platform WebGl

#if !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;

public class Vibration
{
    public static void VibrateMs(int ms)
    {
#if !UNITY_EDITOR
            Vibrate(ms);
#else
        Debug.LogWarning($"Vibration ms: {ms}");
#endif
    }

#if !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void Vibrate(int ms);
#endif
}

#elif Platform_Mobile && UNITY_ANDROID /// Platform Android

using System;
using UnityEngine;

public class Vibration
{

    public static void VibrateMs(int ms)
    {
        //#if !UNITY_EDITOR

        try
        {
            using (AndroidJavaObject unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject vibratorService = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
            {
                AndroidJavaClass vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect");
                AndroidJavaObject effect = vibrationEffect.CallStatic<AndroidJavaObject>("createOneShot", ms, vibrationEffect.GetStatic<int>("DEFAULT_AMPLITUDE"));
                vibratorService.Call("vibrate", effect);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Vibration Error: " + ex.Message);
        }

        //#else
        //        Debug.LogWarning($"Vibration ms: {ms}");
        //#endif
    }
}

#else /// Other Platforms

using UnityEngine;

public class Vibration
{
    public static void VibrateMs(int ms)
    {
        Handheld.Vibrate();
    }
}

#endif